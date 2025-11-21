using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Added for Identity
using Microsoft.EntityFrameworkCore;

namespace Contract_Monthly_Claim_System.Data
{
    // FIX: Inherit from IdentityDbContext instead of DbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // Existing configuration: Convert Enum to String
            mb.Entity<Claim>().Property(c => c.Status).HasConversion<string>().HasMaxLength(50);

            // Existing configuration: Precision for HoursWorked (from previous fix)
            mb.Entity<Claim>().Property(c => c.HoursWorked).HasPrecision(18, 4);

            // Optional: Precision for HourlyRate (good practice)
            mb.Entity<Claim>().Property(c => c.HourlyRate).HasPrecision(18, 2);
        }
    }
}