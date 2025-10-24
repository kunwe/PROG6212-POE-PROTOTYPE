using System;
using System.Collections.Generic;

namespace PROG6212_POE_PROTOTYPE.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; } // Foreign Key to User
        public string LecturerName { get; set; }
        public DateTime ClaimDate { get; set; }
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public DateTime DateSubmitted { get; set; }
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
        public List<Feedback> Feedback { get; set; } = new List<Feedback>();
        public DateTime DateProcessed { get; internal set; }
        public DateTime ClaimMonth { get; set; }
        public string Description { get; set; }

        public Claim()
        {
            Status = "Pending";
            DateSubmitted = DateTime.Now;
        }
    }
}
