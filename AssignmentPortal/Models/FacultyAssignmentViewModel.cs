namespace AssignmentPortal.Models
{
    public class FacultyAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = null!;
        public string ClassroomName { get; set; } = null!;
        public int SubmissionCount { get; set; }
        public double? AverageGrade { get; set; }
    }
}
