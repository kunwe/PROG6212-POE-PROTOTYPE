using System;

namespace PROG6212_POE_PROTOTYPE.Models
{
    public class SupportingDocument
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; } // Foreign Key to Claim
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
    }
}