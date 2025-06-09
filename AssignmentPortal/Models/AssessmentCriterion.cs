using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class AssessmentCriterion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string CriterionName { get; set; } = null!;

        [StringLength(1000)]
        public string Description { get; set; } = null!;

        public double MaxScore { get; set; }

    }
}
