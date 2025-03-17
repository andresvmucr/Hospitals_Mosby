using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;

namespace ProyectoBasesDatos.Controllers
{
    public class UsersController : Controller
    {
        private readonly dbContext _context;

        public UsersController(dbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var dbContext = _context.Users.Include(u => u.Hospital);
            return View(await dbContext.ToListAsync());
        }

        public async Task<IActionResult> Admins()
        {
            var adminUsers = await _context.Users
           .Include(u => u.Hospital)
           .Where(u => u.Role == "admin")
           .ToListAsync();

            return View(adminUsers);
        }

        public async Task<IActionResult> Patients()
        {

            var hospitalId = HttpContext.Session.GetString("IdHospital");
            Console.WriteLine("Hospital id:" + hospitalId);
            var patientsUsers = await _context.Users
           .Include(u => u.Hospital)
           .Where(u => u.Role == "patient" && u.HospitalId == hospitalId)
           .ToListAsync();

            return View(patientsUsers);
        }

        public async Task<IActionResult> Doctors()
        {
            var hospitalId = HttpContext.Session.GetString("IdHospital");

            // Imprimir en el log para verificación
            Console.WriteLine($"Buscando doctores para hospital ID: {hospitalId}");

            // Consulta corregida
            var doctorsUsers = await _context.Doctors
                .Include(d => d.IdDoctorNavigation) // Include the entire navigation property
                .Include(d => d.Specialty) // Include Specialty (not its scalar properties)
                .Where(d => d.IdDoctorNavigation.Role.ToLower() == "doctor" && d.IdDoctorNavigation.HospitalId == hospitalId)
                .Select(d => new
                {
                    Doctor = d,
                    SpecialtyName = d.Specialty != null ? d.Specialty.SpeName : "No especificada"
                })
                .ToListAsync();

            // Verificar resultados
            Console.WriteLine($"Número de doctores encontrados: {doctorsUsers.Count}");

            return View(doctorsUsers);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Role == "doctor")
            {
                var doctor = await _context.Doctors
                    .Where(d => d.IdDoctor == id)
                    .FirstOrDefaultAsync();

                if (doctor != null)
                {
                    var specialty = await _context.Specialties
                        .Where(s => s.Id == doctor.SpecialtyId)
                        .ToListAsync();

                    var workSchedule = await _context.WorkSchedules
                        .Where(w => w.DoctorId == id)
                        .Select(w => new
                        {
                            w.WDay,
                            Starthour = w.Starthour.ToString("HH:mm"), // Formatear a HH:mm
                            Endhour = w.Endhour.ToString("HH:mm")       // Formatear a HH:mm
                        })
                        .ToListAsync();

                    ViewBag.Specialty = specialty;
                    ViewBag.WorkSchedule = workSchedule;
                }
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create(string role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.PredefinedRole = role;
            }

            if (role == "doctor")
            {
                ViewData["Specialties"] = new SelectList(_context.Specialties, "Id", "SpeName");
            }
            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Id");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,UName,Firstlastname,Secondlastname,Birthdate,Password,Gender,Address,Phone,HospitalId,Role")] User user,
    string Specialty, TimeSpan StartTime, TimeSpan EndTime, List<string> WorkingDays) // Recibe horario y días de trabajo
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("UserRole: " + user.Role);
                Console.WriteLine("Specialty: " + Specialty);
                _context.Add(user);
                await _context.SaveChangesAsync();

                if (user.Role == "admin")
                {
                    return RedirectToAction(nameof(Admins));
                }
                else if (user.Role == "patient")
                {
                    return RedirectToAction(nameof(Patients));
                }
                else if (user.Role == "doctor")
                {
                    // Crear el registro del doctor
                    var doctor = new Doctor
                    {
                        Id = user.Id,
                        SpecialtyId = Specialty,
                        IdDoctor = user.Id
                    };
                    _context.Add(doctor);
                    await _context.SaveChangesAsync();


                    foreach (var day in WorkingDays)
                    {
                        var doctorworkingday = new WorkSchedule
                        {
                            WDay = day,
                            Starthour = DateTime.Today.Add(StartTime),
                            Endhour = DateTime.Today.Add(EndTime),
                            DoctorId = doctor.Id
                        };
                        _context.Add(doctorworkingday);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Doctors));
                }
                else
                {
                    return RedirectToAction(nameof(Patients));
                }
            }

            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Id", user.HospitalId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Id", user.HospitalId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UName,Firstlastname,Secondlastname,Birthdate,Password,Gender,Address,Phone,HospitalId,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            // Editing patient
            if (user.Role == "patient")
            {
                var patient = await _context.Users
                    .Include(u => u.Hospital)
                    .FirstOrDefaultAsync(m => m.Id == id);

                // Update the tracked entity's properties
                patient.UName = user.UName;
                patient.Firstlastname = user.Firstlastname;
                patient.Secondlastname = user.Secondlastname;
                patient.Birthdate = user.Birthdate;
                patient.Gender = user.Gender;
                patient.Address = user.Address;
                patient.Phone = user.Phone;
                patient.HospitalId = user.HospitalId;
                patient.Role = user.Role;

                // Save changes
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Patients));
            }
            // Editing doctor
            return RedirectToAction(nameof(Doctors));

        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
