using System.ComponentModel.DataAnnotations;

namespace JobTracker.api.Data
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool EmailConfirmed { get; set; } = false;

        public string? EmailConfirmTokenHash { get; set; }
        public DateTime? EmailConfirmTokenExpiresAtUtc { get; set; }
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetTokenExpiresAtUtc { get; set; }



        public List<JobApplication> JobApplications { get; set; } = new();
    }
}
