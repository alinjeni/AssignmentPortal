namespace AssignmentPortal.Models
{
    public class AssessSubmissionViewModel
    {
        public int AssignmentId { get; set; }
        public int SubmissionId { get; set; }
        public List<AssessedCriterion> Criteria { get; set; } = new();
        public string? Feedback { get; set; }
    }
}
