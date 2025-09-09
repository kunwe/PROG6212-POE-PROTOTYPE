namespace PROG6212_POE_PROTOTYPE.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Lecturer", "Coordinator", "Manager"
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
