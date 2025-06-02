using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!; // e.g., "Student", "Teacher", "Admin"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Submission>? Submissions { get; set; }
        public ICollection<Assignment>? CreatedAssignments { get; set; }
    }
}
