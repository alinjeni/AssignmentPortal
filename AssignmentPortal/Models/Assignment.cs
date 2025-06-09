using System.ComponentModel.DataAnnotations;

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
        public int CreatedById { get; set; }
        public int ClassroomId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;

    }
}
