using System;

namespace PROG6212_POE_PROTOTYPE.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; } // Foreign Key to User
        public DateTime ClaimDate { get; set; }
        public int HoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // "Submitted", "ApprovedByCoordinator", "ApprovedByManager", "Rejected"
        public DateTime DateSubmitted { get; set; }
    }
}