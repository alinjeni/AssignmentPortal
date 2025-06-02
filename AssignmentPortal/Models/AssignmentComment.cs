using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssignmentPortal.Models
{
    public class AssignmentComment
    {
        public int Id { get; set; } // Unique identifier for the comment
        public int AssignmentId { get; set; } // Foreign key to Assignment
        public Assignment Assignment { get; set; } = null!; // Navigation property to Assignment
        public int UserId { get; set; } // Foreign key to User (comment author)
        public User User { get; set; } = null!; // Navigation property to User (comment author)
        [StringLength(1000)]
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = null!; // Content of the comment
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of comment creation
        public DateTime? UpdatedAt { get; set; } // Nullable to allow for comments that haven't been updated
    }
}
