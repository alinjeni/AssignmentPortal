namespace AssignmentPortal.Models
{
    public class ManageClassroomStudentsViewModel
    {
        public Classroom Classroom { get; set; } = null!;
        public List<User> AvailableStudents { get; set; } = new();
        public List<User> CurrentStudents { get; set; } = new();
    }
}
