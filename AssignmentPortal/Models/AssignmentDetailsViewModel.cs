namespace AssignmentPortal.Models
{
    public class AssignmentDetailsViewModel
    {
        public Assignment Assignment { get; set; } = null!;
        public IEnumerable<AssignmentFile> Files { get; set; } = new List<AssignmentFile>();
        public IEnumerable<AssignmentComment> Comments { get; set; } = new List<AssignmentComment>();
        public bool HasSubmitted { get; set; }
        public int? SubmissionId { get; set; }
        public string UserRole { get; set; } = null!;
    }
}
