using System;

namespace PROG6212_POE_PROTOTYPE.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int ClaimId { get; set; } // Foreign Key to Claim
        public int UserId { get; set; }  // Foreign Key to the User (Manager/Coordinator) who wrote the feedback
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime DateAdded { get; set; }

        public Feedback()
        {
            DateAdded = DateTime.Now;
        }
    }
}
