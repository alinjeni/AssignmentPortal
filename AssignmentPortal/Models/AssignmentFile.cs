using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class AssignmentFile
    {
        public int Id { get; set; } // Unique identifier for the file
        public int AssignmentId { get; set; } // Foreign key to Assignment
        public Assignment Assignment { get; set; } = null!; // Navigation property to Assignment
        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = null!; // Name of the file
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = null!; // Path where the file is stored
        public long? FileSize { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow; // Timestamp of file upload
        public int UploadedById { get; set; } // User ID of the uploader
        public User UploadedBy { get; set; } = null!; // Navigation property to User (uploader)
    }
}
