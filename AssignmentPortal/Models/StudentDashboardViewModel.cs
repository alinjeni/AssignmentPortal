namespace AssignmentPortal.Models
{
    public class StudentDashboardViewModel
    {
        public List<string> AssignmentTitles { get; set; } = new();
        public List<double> AssignmentGrades { get; set; } = new();
        public int SubmittedCount { get; set; }
        public int PendingCount { get; set; }
        public List<string> ProgressDates { get; set; } = new();
        public List<double> ProgressGrades { get; set; } = new();
        public List<double> ProgressScores { get; set; } = new();
        public List<int> PerformanceBuckets { get; set; } = new();

    }
}
