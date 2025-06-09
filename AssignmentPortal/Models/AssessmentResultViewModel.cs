namespace AssignmentPortal.Models
{
    public class AssessmentResultViewModel
    {
        public string CriterionName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double MaxScore { get; set; }
        public double Score { get; set; }
        public string Remarks { get; set; } = null!;
    }
}
