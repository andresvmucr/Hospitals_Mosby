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
    public class MedicationsController : Controller
    {
        private readonly dbContext _context;

        public MedicationsController(dbContext context)
        {
            _context = context;
        }

        // GET: Medications
        public async Task<IActionResult> Index()
        {
            var hospitalId = HttpContext.Session.GetString("IdHospital");

            // Obtener las medicinas que pertenecen al hospital específico
            var medications = await _context.Medications
                .Include(m => m.HospitalMeds) // Incluir la relación con HospitalMeds
                .Where(m => m.HospitalMedsId.StartsWith(hospitalId)) // Filtrar por hospitalId
                .ToListAsync();

            return View(medications);
        }

        // GET: Medications/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.HospitalMeds) // Incluir la relación con HospitalMeds
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }


        public async Task<string> GenerateId()
        {
            var medication = await _context.Medications
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextId = 0;
            string newId = $"MED{nextId:D3}";
            if (medication == null)
            {
                Console.WriteLine("New medication id: " + newId);
                return newId;
            }
            string lastId = medication.Id;
            string number = lastId.Substring(3);
            if (int.TryParse(number, out int lastNumber))
            {
                nextId = lastNumber + 1;
            }
            newId = $"MED{nextId:D3}";
            Console.WriteLine("New medication id: " + newId);
            return newId;
        }
        // GET: Medications/Create
        public async Task<IActionResult> Create()
        {
            string newId = await GenerateId();
            ViewData["GeneratedId"] = newId;
            return View();
        }

        // POST: Medications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MName,MDescription")] Medication medication, // Datos de Medication
                                                int price, // Precio desde el formulario
                                                int stock) // Stock desde el formulario
        {
            Console.WriteLine("Id: " + medication.Id);
            Console.WriteLine("MName: " + medication.MName);
            Console.WriteLine("MDescription: " + medication.MDescription);
            Console.WriteLine("Price: " + price);
            Console.WriteLine("Stock: " + stock);
           
            // Crear la entidad HospitalMeds
            var hospitalMedId = HttpContext.Session.GetString("IdHospital") + "_" + medication.Id;
            var hospitalMeds = new HospitalMed
            {
                Id = hospitalMedId,
                Price = price,
                Stock = stock,
                HospitalId = HttpContext.Session.GetString("IdHospital") // Obtener el hospitalId de la sesión
            };

            // Asignar HospitalMedsId a Medication
            medication.HospitalMedsId = hospitalMeds.Id;

            // Guardar ambas entidades en la base de datos
            _context.Add(hospitalMeds);
            await _context.SaveChangesAsync();
            _context.Add(medication);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: Medications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtener el medicamento y sus datos relacionados (HospitalMeds)
            var medication = await _context.Medications
                .Include(m => m.HospitalMeds) // Incluir la relación con HospitalMeds
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null)
            {
                return NotFound();
            }

            // Pasar los datos a la vista
            ViewBag.Price = medication.HospitalMeds.Price; // Precio
            ViewBag.Stock = medication.HospitalMeds.Stock; // Stock

            return View(medication);
        }

        // POST: Medications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,MName,MDescription")] Medication medication, int price, int stock)
        {
            if (id != medication.Id)
            {
                return NotFound();
            }
            Console.WriteLine("Id: " + medication.Id);
            Console.WriteLine("MName: " + medication.MName);
            Console.WriteLine("MDescription: " + medication.MDescription);
            Console.WriteLine("Price: " + price);
            Console.WriteLine("Stock: " + stock);

            try
            {
                // Obtener el medicamento existente y sus datos relacionados
                var existingMedication = await _context.Medications
                    .Include(m => m.HospitalMeds)
                    .FirstOrDefaultAsync(m => m.Id == id);

                // Actualizar los campos del medicamento
                existingMedication.MName = medication.MName;
                existingMedication.MDescription = medication.MDescription;

                // Actualizar los campos de HospitalMeds
                if (existingMedication.HospitalMeds != null)
                {
                    existingMedication.HospitalMeds.Price = price;
                    existingMedication.HospitalMeds.Stock = stock;
                    _context.Update(existingMedication.HospitalMeds);
                    await _context.SaveChangesAsync();// Guardar los cambios en la base de datos
                }

                _context.Update(existingMedication);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicationExists(medication.Id))
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


        private bool MedicationExists(string id)
        {
            return _context.Medications.Any(e => e.Id == id);
        }
    }
}
