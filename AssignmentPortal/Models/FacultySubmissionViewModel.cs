namespace AssignmentPortal.Models
{
    public class FacultySubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; } = null!;
        public string AssignmentTitle { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public double? TotalGrade { get; set; }
    }
}
