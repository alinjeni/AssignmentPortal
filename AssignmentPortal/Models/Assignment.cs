using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssignmentPortal.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; } // User ID of the creator
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; } = null!; // Navigation property to User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } // Nullable to allow for assignments that haven't been updated
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>(); // Navigation property for submissions
        public ICollection<AssignmentFile> AssignmentFiles { get; set; } = new List<AssignmentFile>(); // Navigation property for files
        public ICollection<AssignmentComment> Comments { get; set; } = new List<AssignmentComment>(); // Navigation property for comments
        public bool IsActive { get; set; } = true; // Indicates if the assignment is active or archived

    }
}
