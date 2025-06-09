namespace AssignmentPortal.Models
{
    public class AssignmentWithAvgGrade
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public double? AverageGrade { get; set; }
    }
}
