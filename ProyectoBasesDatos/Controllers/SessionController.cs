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
            return View();
        }
    }
}
