using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class AssignmentComment
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        [StringLength(1000)]
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
