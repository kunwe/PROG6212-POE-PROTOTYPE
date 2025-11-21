using System.Linq;
using System.Threading.Tasks;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.services.interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization; // Required

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize] // Restricts access to logged-in users only
    public class AdminController : Controller
    {
        private readonly IClaimService _claimService;

        public AdminController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        // GET: /Admin (Lists all pending claims)
        public async Task<IActionResult> Index()
        {
            var pendingClaims = await _claimService.GetAllPendingAsync();
            return View(pendingClaims);
        }

        // POST: /Admin/Approve/{id}
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var approvedBy = User.Identity?.Name ?? "Admin";
            try
            {
                await _claimService.ApproveAsync(id, approvedBy);
                TempData["Success"] = $"Claim ID {id} has been Approved.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Approval failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Reject/{id}
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var rejectedBy = User.Identity?.Name ?? "Admin";

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Rejection reason is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _claimService.RejectAsync(id, rejectedBy, reason);
                TempData["Success"] = $"Claim ID {id} has been Rejected.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rejection failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}