using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;

namespace ProyectoBasesDatos.Controllers
{
    public class TreatmentsController : Controller
    {
        private readonly dbContext _context;

        public TreatmentsController(dbContext context)
        {
            _context = context;
        }

        [HttpGet("Appointments/Perform/{id}/GetMeds")]
        public IActionResult GetMeds(string id)
        {
            var hospital = HttpContext.Session.GetString("IdHospital");

            var medications = _context.Medications
                .Where(m => m.HospitalMedsId.StartsWith(hospital))
                .Select(m => new { id = m.Id, medName = m.MName })
                .ToList();

            return Json(medications);
        }

        // GET: Treatments
        public async Task<IActionResult> Index()
        {
            var dbContext = _context.Treatments.Include(t => t.Appointment);
            return View(await dbContext.ToListAsync());
        }

        // GET: Treatments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Appointment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        public async Task<string> GenerateTreatmentID()
        {
            var tratamientos = await _context.Treatments
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextID = 0;

            if (tratamientos != null)
            {
                string lastID = tratamientos.Id;
                string number = lastID.Substring(3);
                if (int.TryParse(number, out int lastNumber))
                {
                    nextID = lastNumber + 1;
                }

            }

            string newId = $"TRM{nextID:D3}";
            return newId;
        }

        public async Task<string> GenerateID()
        {
            var treatmentMed = await _context.TreatmentMeds
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextID = 0;

            if (treatmentMed != null)
            {
                string lastID = treatmentMed.Id;
                string number = lastID.Substring(3);
                if (int.TryParse(number, out int lastNumber))
                {
                    nextID = lastNumber + 1;
                }

            }
            string newId = $"TMD{nextID:D3}";
            return newId;
        }

        // GET: Treatments/Create
        public IActionResult Create()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "Id", "Id");
            return View();
        }

        // POST: Treatments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string AppointmentId, List<string> TreatmentMeds, List<int> Quantity, List<string> Frequency)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Console.WriteLine("TreatmentsMeds: " + string.Join(", ", TreatmentMeds));
                Console.WriteLine("Quantity: " + string.Join(", ", Quantity));
                Console.WriteLine("Frequency: " + string.Join(", ", Frequency));
                Console.WriteLine("Parameter Id: " + AppointmentId);
                var appointment = await _context.Appointments.FindAsync(AppointmentId);

                if (appointment == null)
                {
                    return NotFound();
                }

                var treatments = TreatmentMeds.Zip(Quantity, (med, cant) => new { Medicines = med, Stock = cant })
                                                   .Zip(Frequency, (trat, frec) => new { trat.Medicines, trat.Stock, Frequency = frec });

                int totalPrice = 0;

                foreach (var medicines in treatments)
                {
                    var med = await _context.Medications.FindAsync(medicines.Medicines);
                    Console.WriteLine("Medication: " + med.MName);
                    if (med == null)
                    {
                        return BadRequest($"El medicamento con ID {medicines.Medicines} no existe.");
                    }

                    var hospitalMed = await _context.HospitalMeds.FindAsync(med.HospitalMedsId);
                    totalPrice += (int)(hospitalMed.Price * medicines.Stock);
                    Console.WriteLine("Total price: " + totalPrice);
                }

                var treatment = new Treatment
                {
                    Id = await GenerateTreatmentID(),
                    Price = totalPrice,
                    AppointmentId = AppointmentId,
                };

                _context.Treatments.Add(treatment);
                await _context.SaveChangesAsync();

                foreach (var meds_treatment in treatments)
                {
                    var treatmentMed = new TreatmentMed
                    {
                        Id = await GenerateID(),
                        Dosis = meds_treatment.Stock.ToString(),
                        Frequency = meds_treatment.Frequency,
                        TDate = appointment.ADay,
                        TreatmentId = treatment.Id,
                        MedicationId = meds_treatment.Medicines
                    };
                    Console.WriteLine("Adding " + treatmentMed.Id + " to table");
                    _context.TreatmentMeds.Add(treatmentMed);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();



                appointment.AStatus = "Completed";
                _context.Update(appointment);
                await _context.SaveChangesAsync();


                return RedirectToAction("DoctorHome", "Home");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {

                    string errorMessage = sqlEx.Message;


                    int firstDotIndex = errorMessage.IndexOf('.');
                    if (firstDotIndex > 0)
                    {
                        errorMessage = errorMessage.Substring(0, firstDotIndex);
                    }


                    TempData["ErrorMessage"] = errorMessage;


                    await transaction.RollbackAsync();
                    return RedirectToAction("Perform", "Appointments", new { id = AppointmentId });
                }

                await transaction.RollbackAsync();
                throw;
            }
        }

        // GET: Treatments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "Id", "Id", treatment.AppointmentId);
            return View(treatment);
        }

        // POST: Treatments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Price,AppointmentId")] Treatment treatment)
        {
            if (id != treatment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.Id))
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
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "Id", "Id", treatment.AppointmentId);
            return View(treatment);
        }

        // GET: Treatments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Appointment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment != null)
            {
                _context.Treatments.Remove(treatment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(string id)
        {
            return _context.Treatments.Any(e => e.Id == id);
        }
    }
}
