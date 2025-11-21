// Contract_Monthly_Claim_System.Models/Claim.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contract_Monthly_Claim_System.Models
{
    public class Claim
    {
        // 🌟 FIX: Parameterless constructor for Entity Framework Core
        public Claim() { }

        // Example custom constructor (can be kept, but EF Core will ignore it for materialization)
        // Note: The parameters 'name' and 'testUserEmail' mentioned in your original error 
        // are likely from other constructors you didn't show or were testing.
        public Claim(DateTime period, string lecturerName)
        {
            ClaimPeriod = period;
            LecturerName = lecturerName;
            ClaimStatus = ClaimStatus.Draft; // Initialize status
        }

        // --- Properties ---

        [Key]
        public int ClaimId { get; set; }

        [Required]
        [Display(Name = "Claim Period")]
        [DataType(DataType.Date)]
        // Ensures only the month and year are relevant
        [DisplayFormat(DataFormatString = "{0:MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ClaimPeriod { get; set; }

        // Mapped property for the lecturer's unique identifier (email)
        [Required]
        [StringLength(256)]
        public string LecturerName { get; set; } // Renamed from LecturerEmail for clarity/consistency

        [Required]
        [Range(0.01, 1000.0)] // Assuming reasonable range for hours
        [Display(Name = "Hours Worked")]
        public double HoursWorked { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 5000.0)] // Assuming reasonable hourly rate
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        // Calculate total amount on the fly
        [NotMapped]
        public decimal TotalAmount => (decimal)HoursWorked * HourlyRate;

        [StringLength(500)]
        [Display(Name = "Notes/Remarks")]
        public string Notes { get; set; }

        public ClaimStatus ClaimStatus { get; set; } = ClaimStatus.Draft; // Default status

        // --- Navigation Properties ---
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }

    public enum ClaimStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        Paid
    }

    // (You would need a simple Attachment model here too)
    public class Attachment
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; } // Where the file is stored (e.g., in a blob service or file system)
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }

        // Navigation Property
        public Claim Claim { get; set; }
    }
}
