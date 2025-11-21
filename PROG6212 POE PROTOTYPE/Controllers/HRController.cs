using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Added for User Management

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // Inject UserManager

        public HRController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerateReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved)
                .OrderBy(c => c.LecturerName)
                .ToListAsync();

            ViewData["GrandTotal"] = approvedClaims.Sum(c => c.Amount);
            ViewData["GeneratedDate"] = DateTime.Now;

            return View(approvedClaims);
        }

        // === NEW: Lecturer Management Actions ===

        // GET: /HR/Lecturers
        // Lists all registered users (Lecturers)
        public IActionResult Lecturers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: /HR/EditLecturer/{id}
        public async Task<IActionResult> EditLecturer(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /HR/EditLecturer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(string id, string email, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Update fields
            user.Email = email;
            user.UserName = email; // Keep username consistent with email
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Lecturer details updated successfully.";
                return RedirectToAction(nameof(Lecturers));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(user);
        }
    }
}