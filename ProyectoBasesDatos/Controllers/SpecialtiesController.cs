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
    public class SpecialtiesController : Controller
    {
        private readonly dbContext _context;

        public SpecialtiesController(dbContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialties.ToListAsync());
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        public async Task<string> GenerateId()
        {
            var specialty = await _context.Specialties
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            var nextId = 0;

            string newId = $"ESP{nextId:D3}";
            if (specialty == null)
            {
                Console.WriteLine("New specialty id: " + newId);
                return newId;

            }

            string lastId = specialty.Id;
            string number = lastId.Substring(3);
            if (int.TryParse(number, out int lastNumber))
            {
                nextId = lastNumber + 1;
            }

            newId = $"ESP{nextId:D3}";
            Console.WriteLine("New specialty id: " + newId);
            return newId;

        }

        // GET: Specialties/Create
        public async Task<IActionResult> Create()
        {
            string newId = await GenerateId();
            ViewData["GeneratedId"] = newId;
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpeName,Description")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialty);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SpeName,Description")] Specialty specialty)
        {
            if (id != specialty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
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
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
                _context.Specialties.Remove(specialty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialtyExists(string id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
