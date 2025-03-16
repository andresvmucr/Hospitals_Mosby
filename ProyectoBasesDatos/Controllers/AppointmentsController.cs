using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;

namespace ProyectoBasesDatos.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly dbContext _context;

        public AppointmentsController(dbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                // Unimos la cita con el usuario que es paciente
                .Join(_context.Users,
                      a => a.PatientId, // Relacionando paciente en la cita
                      p => p.Id,
                      (a, p) => new { Appointment = a, Patient = p })
                // Unimos la cita con el usuario que es doctor
                .Join(_context.Users,
                      ap => ap.Appointment.DoctorId, // Relacionando doctor en la cita
                      d => d.Id,
                      (ap, d) => new { ap.Appointment, ap.Patient, DoctorUser = d })
                // Unimos con la tabla Doctors para obtener la especialidad
                .Join(_context.Doctors,
                      ad => ad.DoctorUser.Id, // Relacionando el usuario doctor con Doctors
                      doc => doc.IdDoctor,
                      (ad, doc) => new { ad.Appointment, ad.Patient, ad.DoctorUser, Doctor = doc })
                // Unimos con la tabla Specialties para obtener el nombre de la especialidad
                .Join(_context.Specialties,
                      ado => ado.Doctor.SpecialtyId,
                      s => s.Id,
                      (ado, s) => new
                      {
                          Appointment = ado.Appointment,
                          DoctorFullName = $"{ado.DoctorUser.UName} {ado.DoctorUser.Firstlastname} {ado.DoctorUser.Secondlastname}",
                          PatientFullName = $"{ado.Patient.UName} {ado.Patient.Firstlastname} {ado.Patient.Secondlastname}",
                          SpecialtyName = s.SpeName
                      })
                .ToListAsync();

            return View(appointments);
        }

        public async Task<string> GenerateId()
        {
            var appointment = await _context.Appointments
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextId = 0;
            string newId = $"APP{nextId:D3}";
            if (appointment == null)
            {
                Console.WriteLine("New medication id: " + newId);
                return newId;
            }
            string lastId = appointment.Id;
            string number = lastId.Substring(3);
            if (int.TryParse(number, out int lastNumber))
            {
                nextId = lastNumber + 1;
            }
            newId = $"APP{nextId:D3}";
            Console.WriteLine("New appointment id: " + newId);
            return newId;
        }


        public async Task<IActionResult> GetDoctorsBySpecialty(string specialtyId)
        {
            try
            {
                Console.WriteLine("Consultando para: " + specialtyId);
                var hospitalId = "H000"; // Obtén el ID del hospital desde la sesión o donde corresponda
                var users = await _context.Users.ToListAsync();
                var doctores = await _context.Doctors
                    .Where(d => d.SpecialtyId == specialtyId && d.IdDoctorNavigation.HospitalId == hospitalId)
                    .Select(d => new
                    {
                        d.Id,
                        fullName = d.IdDoctorNavigation.UName + " " + d.IdDoctorNavigation.Firstlastname + " " + d.IdDoctorNavigation.Secondlastname
                    })
                    .ToListAsync();

                // Imprimir los resultados en la consola
                foreach (var doctor in doctores)
                {
                    Console.WriteLine($"ID: {doctor.Id}, Nombre Completo: {doctor.fullName}");
                }

                return Json(doctores);
            }
            catch (Exception ex)
            {
                // Imprimir el error en la consola
                Console.WriteLine("Error en controller: " + ex.Message);

                // Devolver un mensaje de error en formato JSON
                return Json(new { error = ex.Message });
            }
        }


        public async Task<IActionResult> GetAvailableHours(string idDoctor, string day)
        {
            Console.WriteLine("Id: " + idDoctor);
            Console.WriteLine("Day: " + day);
            DateTime date = DateTime.Parse(day);
            string[] weekDays = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            string daysOfTheWeek = weekDays[(int)date.DayOfWeek];

            Console.WriteLine("days of the week: " + daysOfTheWeek);

            var schedules = await _context.WorkSchedules
                .Where(h => h.DoctorId == idDoctor && h.WDay == daysOfTheWeek)
                .Select(h => new { h.Starthour, h.Endhour })
                .ToListAsync();

            List<string> availableHours = new List<string>();

            foreach (var schedule in schedules)
            {
                Console.WriteLine("Schedule" + schedule);
                TimeSpan beginHour = schedule.Starthour.TimeOfDay; // Extrae solo la hora si es DATETIME
                TimeSpan endHour = schedule.Endhour.TimeOfDay;     // Extrae solo la hora si es DATETIME

                if (beginHour < endHour)
                {
                    for (TimeSpan hour = beginHour; hour < endHour; hour = hour.Add(TimeSpan.FromMinutes(60)))
                    {
                        Console.WriteLine("Adding " + hour.ToString(@"hh\:mm"));
                        availableHours.Add(hour.ToString(@"hh\:mm"));
                    }
                }
                else
                {
                    for (TimeSpan hour = beginHour; hour < TimeSpan.FromHours(24); hour = hour.Add(TimeSpan.FromMinutes(60)))
                    {
                        Console.WriteLine("Adding " + hour.ToString(@"hh\:mm"));
                        availableHours.Add(hour.ToString(@"hh\:mm"));
                    }

                    for (TimeSpan hour = TimeSpan.Zero; hour < endHour; hour = hour.Add(TimeSpan.FromMinutes(60)))
                    {
                        Console.WriteLine("Adding " + hour.ToString(@"hh\:mm"));
                        availableHours.Add(hour.ToString(@"hh\:mm"));
                    }
                }
            }
            Console.WriteLine("AVAILABLE HOURS: " + string.Join(", ", availableHours));
            return Json(availableHours);
        }





        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Specialty"] = new SelectList(_context.Specialties, "Id", "SpeName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ADay,AHour,PatientId,DoctorId")] Appointment appointment)
        {
            try
            {
                appointment.Id = await GenerateId();
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("RegisterAppointment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AppointmentId", appointment.Id);
                        command.Parameters.AddWithValue("@AppointmentDay", appointment.ADay);
                        command.Parameters.AddWithValue("@AppointmentHour", appointment.AHour);
                        command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                        command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                        command.Parameters.AddWithValue("@AStatus", appointment.AStatus);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                if (HttpContext.Session.GetString("Role") == "admin")
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("PatientHome", "Home");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error SQL: " + ex.Message);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(appointment);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar la cita.");
                return View(appointment);
            }
        }



        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", appointment.PatientId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ADay,AHour,AStatus,PatientId,DoctorId")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", appointment.PatientId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(string id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
