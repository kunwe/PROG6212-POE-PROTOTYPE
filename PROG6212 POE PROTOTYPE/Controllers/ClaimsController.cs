using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PROG6212_POE_PROTOTYPE.Models;

namespace PROG6212_POE_PROTOTYPE.Controllers
{
    public class ClaimsController : Controller
    {
        // In-memory storage for demo purposes
        private static List<Claim> _claims = new List<Claim>();
        private static int _nextClaimId = 1;
        private static int _nextDocumentId = 1;
        private static int _nextFeedbackId = 1;

        private readonly string[] _allowedFileTypes = { ".pdf", ".docx", ".xlsx", ".doc", ".xls", ".png", ".jpg", ".jpeg" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Claim claim, List<IFormFile> supportingDocuments)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Calculate total amount
                    claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;
                    claim.ClaimId = _nextClaimId++;
                    claim.UserId = 1; // Mock user ID - in real app, get from logged-in user
                    claim.LecturerName = "Dr. Sample Lecturer"; // Mock name
                    claim.DateSubmitted = DateTime.Now;
                    claim.Status = "Pending";

                    // Handle file uploads
                    if (supportingDocuments != null && supportingDocuments.Count > 0)
                    {
                        foreach (var file in supportingDocuments)
                        {
                            if (file.Length > 0)
                            {
                                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                                // Validate file type
                                if (!_allowedFileTypes.Contains(fileExtension))
                                {
                                    ModelState.AddModelError("supportingDocuments", $"File type {fileExtension} is not allowed. Allowed types: PDF, DOCX, XLSX, PNG, JPG");
                                    return View(claim);
                                }

                                // Validate file size
                                if (file.Length > _maxFileSize)
                                {
                                    ModelState.AddModelError("supportingDocuments", $"File {file.FileName} exceeds the maximum allowed size of 5MB");
                                    return View(claim);
                                }

                                // Create document object
                                var document = new SupportingDocument
                                {
                                    DocumentId = _nextDocumentId++,
                                    ClaimId = claim.ClaimId,
                                    FileName = file.FileName,
                                    FileSize = file.Length,
                                    ContentType = file.ContentType
                                };

                                // In a real application, you would save the file to a secure location
                                // For demo purposes, we'll just store the file info
                                var filePath = $"/uploads/{claim.ClaimId}_{file.FileName}";
                                document.FilePath = filePath;

                                claim.SupportingDocuments.Add(document);
                            }
                        }
                    }

                    _claims.Add(claim);
                    TempData["SuccessMessage"] = "Claim submitted successfully!";
                    return RedirectToAction("Dashboard", "Home", new { role = "Lecturer" });
                }
                return View(claim);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while submitting the claim. Please try again.");
                return View(claim);
            }
        }

        // Action for coordinators/managers to view all pending claims
        public IActionResult Review()
        {
            var pendingClaims = _claims.Where(c => c.Status == "Pending").ToList();
            return View(pendingClaims);
        }

        // Action for approving a claim
        [HttpPost]
        public IActionResult Approve(int claimId)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim != null)
                {
                    claim.Status = "Approved";

                    // Add feedback
                    var feedback = new Feedback
                    {
                        FeedbackId = _nextFeedbackId++,
                        ClaimId = claimId,
                        UserId = 2, // Mock coordinator/manager ID
                        UserName = "Coordinator",
                        Comment = "Claim approved successfully."
                    };
                    claim.Feedback.Add(feedback);

                    return Json(new { success = true, message = "Claim approved successfully" });
                }
                return Json(new { success = false, message = "Claim not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while approving the claim" });
            }
        }

        // Action for rejecting a claim
        [HttpPost]
        public IActionResult Reject(int claimId, string feedback)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim != null)
                {
                    claim.Status = "Rejected";

                    // Add feedback
                    var feedbackObj = new Feedback
                    {
                        FeedbackId = _nextFeedbackId++,
                        ClaimId = claimId,
                        UserId = 2, // Mock coordinator/manager ID
                        UserName = "Coordinator",
                        Comment = feedback
                    };
                    claim.Feedback.Add(feedbackObj);

                    return Json(new { success = true, message = "Claim rejected successfully" });
                }
                return Json(new { success = false, message = "Claim not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while rejecting the claim" });
            }
        }

        // Action to get claim details
        public IActionResult Details(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                return Json(claim);
            }
            return NotFound();
        }

        // Action to get claims for a specific user
        public IActionResult GetUserClaims(int userId)
        {
            var userClaims = _claims.Where(c => c.UserId == userId).ToList();
            return Json(userClaims);
        }
    }
}
