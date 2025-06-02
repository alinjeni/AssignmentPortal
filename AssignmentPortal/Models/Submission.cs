using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; } // Foreign key to Assignment
        public Assignment Assignment { get; set; } = null!; // Navigation property to Assignment
        public int UserId { get; set; } // Foreign key to User
        public User User { get; set; } = null!; // Navigation property to User
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = null!; // Content of the submission
        [DataType(DataType.DateTime)]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow; // Timestamp of submission
        public DateTime? GradedAt { get; set; } // Nullable to allow for ungraded submissions
        public string? Feedback { get; set; } // Optional feedback from the teacher
        public double? TotalGrade { get; set; } // Nullable to allow for ungraded submissions
        public ICollection<SubmissionFile> SubmissionFiles { get; set; } = new List<SubmissionFile>();
    }
}
