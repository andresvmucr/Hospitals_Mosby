using Microsoft.AspNetCore.Mvc;

namespace ProyectoBasesDatos.Controllers
{
    public class AdminsController : Controller
    {
        public IActionResult AdvancedQueriesHome()
        {
            return View();
        }

        public IActionResult PatientMeds()
        {
            return View();
        }

        public IActionResult StockPrescription()
        {
            return View();
        }

        public IActionResult TotalPayment()
        {
            return View();
        }

        public IActionResult PendingPayment()
        {
            return View();
        }

    }
}
