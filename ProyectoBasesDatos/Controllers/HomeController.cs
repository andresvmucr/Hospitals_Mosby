using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;

namespace ProyectoBasesDatos.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly dbContext _context;

    public HomeController(dbContext context,ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult SuperAdminHome()
    {
        Console.WriteLine("IActionResult SuperAdminHome");
        return View();
    }

    public IActionResult AdminHome()
    {
        Console.WriteLine("IActionResult AdminHome");
        return View();
    }

    public async Task<IActionResult> DoctorHome()
    {
        var id = HttpContext.Session.GetString("Id");
        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
                .ThenInclude(d => d.IdDoctorNavigation)
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == id && c.AStatus == "pending")
            .ToListAsync();

        Console.WriteLine("IActionResult DoctorHome");
        return View(appointments);
    }


    public async Task<IActionResult> PatientHome()
    {
        var id = HttpContext.Session.GetString("Id");
        var appointments = await _context.Appointments
        .Include(a => a.Doctor)
            .ThenInclude(d => d.IdDoctorNavigation)
        .Include(c => c.Patient)
        .Include(a => a.Treatments) // Asegúrate de incluir los tratamientos
        .Where(c => c.PatientId == id)
        .ToListAsync();

        return View(appointments);
    }
}
