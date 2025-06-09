using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class AssignmentFile
    {
        public int Id { get; set; } 
        public int AssignmentId { get; set; } 
        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = null!;
        public long? FileSize { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int UploadedById { get; set; }
    }
}
