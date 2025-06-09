namespace AssignmentPortal.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int FacultyId { get; set; }
        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

        public List<AssessedCriterion> AssessedCriteria { get; set; } = new();
    }
}
