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
    public class HospitalsController : Controller
    {
        private readonly dbContext _context;

        public HospitalsController(dbContext context)
        {
            _context = context;
        }

        // GET: Hospitals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hospitals.ToListAsync());
        }

        // GET: Hospitals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }


        public async Task<string> GenerateId()
        {
            var hospital = await _context.Hospitals
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextId = 0;
            string newId = $"HOSP{nextId:D3}";
            if (hospital == null)
            {
                Console.WriteLine("New hospital id: " + newId);
                return newId; 

            }

            string lastId = hospital.Id;
            string number = lastId.Substring(4);
            if (int.TryParse(number, out int lastNumber)) {
                nextId = lastNumber + 1;
            }

            newId = $"HOSP{nextId:D3}";
            Console.WriteLine("New hospitals id: " + newId);
            return newId;
           
        }


        // GET: Hospitals/Create
        public async Task<IActionResult> Create()
        {
            string newId = await GenerateId();
            ViewData["GeneratedId"] = newId;
            return View();
        }

        // POST: Hospitals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HName,HAddress,Phone")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hospital);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hospital);
        }

        // GET: Hospitals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }
            return View(hospital);
        }

        // POST: Hospitals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,HName,HAddress,Phone")] Hospital hospital)
        {
            if (id != hospital.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hospital);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalExists(hospital.Id))
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
            return View(hospital);
        }


        private bool HospitalExists(string id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}
