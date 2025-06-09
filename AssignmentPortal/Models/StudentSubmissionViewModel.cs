namespace AssignmentPortal.Models
{
    public class StudentSubmissionViewModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public double? TotalGrade { get; set; }
        public string Feedback { get; set; } = null!;
    }
}
