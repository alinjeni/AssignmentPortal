namespace AssignmentPortal.Models
{
    public class StudentAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public string ClassroomName { get; set; } = null!;
        public bool IsSubmitted { get; set; }
        public bool IsGraded { get; set; }
    }
}
