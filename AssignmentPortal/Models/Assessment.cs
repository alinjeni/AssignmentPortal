namespace AssignmentPortal.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; } // Foreign key to Submission
        public Submission Submission { get; set; } = null!; // Navigation property to Submission
        public int FacultyId { get; set; } // Foreign key to User (teacher)
        public User Faculty { get; set; } = null!; // Navigation property to User (teacher)
        public DateTime AssessedAt { get; set; } = DateTime.UtcNow; // Timestamp of assessment submission
        public ICollection<AssessmentCriterion> Criteria { get; set; } = new List<AssessmentCriterion>();
    }
}
