using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE_PROTOTYPE.Controllers
{
    public class ClaimsController : Controller
    {


        public IActionResult Create()
        {
            // Display the form to create a new claim
            return View();
        }

        [HttpPost]
        public IActionResult Create(PROG6212_POE_PROTOTYPE.Models.Claim newClaim)
        {

            return RedirectToAction("Dashboard", "Home", new { role = "Lecturer" });
        }
    }
}