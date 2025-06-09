namespace AssignmentPortal.Models
{
    public class SubmissionDetailsViewModel
    {
        public Submission? Submission { get; set; }
        public List<SubmissionFile> Files { get; set; } = new();
        public Assessment? Assessment { get; set; }
        public List<AssessmentResultViewModel> CriteriaResults { get; set; } = new();
    }
}
