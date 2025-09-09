using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_PROTOTYPE.Models;

namespace PROG6212_POE_PROTOTYPE.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index (Default page - the login screen)
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {

            return RedirectToAction("Dashboard", new { role = role });
        }

        // GET: /Home/Dashboard?role=Lecturer
        public IActionResult Dashboard(string role)
        {
            // Pass the role to the view so it can display the correct dashboard.
            ViewBag.UserRole = role;
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: /Home/Register
        // This action handles the form submission.
        [HttpPost] // This attribute is crucial to distinguish this from the GET version
        public IActionResult Register(UserRegistrationModel model)
        {
            TempData["SuccessMessage"] = "Registration successful! Your account is pending approval by an administrator.";
            return RedirectToAction("Index"); // Redirect back to the login page
        }
    }
}
