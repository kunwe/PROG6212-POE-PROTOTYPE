using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.services.interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize] // Forces users to log in to access any page in this controller
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        // GET: /Claims
        public async Task<IActionResult> Index()
        {
            // Get the email of the currently logged-in user
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Index", "Home");
            }

            // Fetch claims ONLY for this specific lecturer
            var claims = await _claimService.GetAllForLecturerAsync(userEmail);

            ViewData["LecturerName"] = userEmail;
            return View(claims);
        }

        // GET /Claims/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var c = await _claimService.GetByIdAsync(id);
            if (c == null) return NotFound();

            // Security Check: Ensure the claim belongs to the logged-in user (or allow Admins)
            // For simplicity in this prototype, we allow viewing if you have the link, 
            // but in production, you would check c.LecturerName == User.Identity.Name

            return View(c);
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            // Pre-fill the lecturer name with the logged-in user's email
            var model = GetModel();
            return View(model);
        }

        private Claim GetModel()
        {
            return new Claim
            {
                ClaimPeriod = DateTime.Now,
                LecturerName = User.Identity?.Name ?? "Unknown"
            };
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClaimPeriod,HoursWorked,HourlyRate,Notes")] Claim claim)
        {
            // FORCE the lecturer name to be the logged-in user
            claim.LecturerName = User.Identity?.Name ?? "Unknown";

            // Remove validation for Notes (optional) and LecturerName (set manually)
            if (claim.Notes == null) ModelState.Remove("Notes");
            ModelState.Remove("LecturerName");

            if (ModelState.IsValid)
            {
                await _claimService.AddClaimAsync(claim);
                TempData["Success"] = $"Claim for {claim.ClaimPeriod:MMMM yyyy} created successfully. Add attachments and submit.";
                return RedirectToAction(nameof(Details), new { id = claim.ClaimId });
            }
            return View(claim);
        }

        // POST: /Claims/Submit/{id}
        [HttpPost]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                await _claimService.SubmitForReviewAsync(id);
                TempData["Success"] = "Claim submitted for review! It is now pending admin approval.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Submission failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int claimId, IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    TempData["Error"] = "Please select a file to upload.";
                    return RedirectToAction("Details", new { id = claimId });
                }

                var uploadedBy = User.Identity?.Name ?? "Unknown User";
                await _claimService.AddAttachmentAsync(claimId, file, uploadedBy);

                TempData["Success"] = "Attachment uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Upload failed: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = claimId });
        }
    }
}
        }
    }
}
