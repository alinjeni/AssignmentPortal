namespace AssignmentPortal.Models
{
    public class FacultyDashboardViewModel
    {
        public List<string> ClassroomNames { get; set; } = new();
        public List<int> AssignmentsPerClassroom { get; set; } = new();
        public List<string> AssignmentTitles { get; set; } = new();
        public List<double> AverageGrades { get; set; } = new();
        public List<string> StudentNames { get; set; } = new();
        public List<int> SubmissionsPerStudent { get; set; } = new();
    }
}
