using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;



namespace ProyectoBasesDatos.Controllers
{
    public class SessionController : Controller
    {
        private readonly dbContext _context;
        public SessionController(dbContext context)
        {
            _context = context;
        }

        // GET: SessionController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Id, string Password)
        {
            Console.WriteLine("ID: " + Id);
            Console.WriteLine("Password: " + Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id && u.Password == Password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            Console.WriteLine("User: " + user.UName);
            Console.WriteLine("Role: " + user.Role);

            HttpContext.Session.SetString("Id", user.Id);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "superadmin")
            {
                return RedirectToAction("SuperAdminHome", "Home");
            } 
            else
            {
                HttpContext.Session.SetString("IdHospital", user.HospitalId);
                if (user.Role == "admin")
                {
                    return RedirectToAction("AdminHome", "Home");
                }
                else if (user.Role == "doctor")
                {
                    return RedirectToAction("DoctorHome", "Home");
                }
                else if (user.Role == "patient")
                {
                    return RedirectToAction("PatientHome", "Home");
                }
            }
            return View();
        }
    }
}
