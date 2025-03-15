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

            // Obtener los doctores con sus especialidades y horarios
            var doctors = await _context.Doctors
                .Include(d => d.Specialty) // Incluir la especialidad del doctor
                .Include(d => d.WorkSchedules) // Incluir los horarios del doctor
                .Include(d => d.User) // Incluir la relación con User
                    .ThenInclude(u => u.Hospital) // Incluir el hospital del usuario
                .Where(d => d.User.HospitalId == hospitalId && d.User.Role == "doctor")
                .ToListAsync();

            return View(doctors);
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

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create(string role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.PredefinedRole = role;
            }

            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Id");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UName,Firstlastname,Secondlastname,Birthdate,Password,Gender,Address,Phone,HospitalId,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("UserRole: " + user.Role);
                _context.Add(user);
                await _context.SaveChangesAsync();

                if (user.Role == "admin")
                {
                    return RedirectToAction(nameof(Admins));
                } else if(user.Role == "patient") {
                    return RedirectToAction(nameof(Patients));
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
