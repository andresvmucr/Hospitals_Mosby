using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoBasesDatos.Models;

namespace ProyectoBasesDatos.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult SuperAdminHome()
    {
        Console.WriteLine("IActionResult SuperAdminHome");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

}
