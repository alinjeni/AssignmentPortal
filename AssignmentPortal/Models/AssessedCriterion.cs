namespace AssignmentPortal.Models
{
    public class AssessedCriterion
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public int CriterionId { get; set; }
        public string CriterionName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double MaxScore { get; set; }
        public double Score { get; set; }
        public string Remarks { get; set; } = null!;
    }
}
