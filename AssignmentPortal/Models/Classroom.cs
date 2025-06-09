using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class Classroom
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByName { get; set; }
        public bool IsArchived { get; set; } = false;
    }
}
