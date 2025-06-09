using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; } 
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = null!;
        [DataType(DataType.DateTime)]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow; 
        public DateTime? GradedAt { get; set; } 
        public string? Feedback { get; set; } 
        public double? TotalGrade { get; set; } 
    }
}
