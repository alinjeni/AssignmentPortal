using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class AssessmentCriterion
    {
        public int Id { get; set; } // Unique identifier for the criterion
        public int AssessmentId { get; set; } // Foreign key to Assessment
        public Assessment Assessment { get; set; } = null!; // Navigation property to Assessment
        [Required]
        [StringLength(200)]
        public string CriterionName { get; set; } = null!; // Name of the assessment criterion
        [StringLength(1000)]
        public string Description { get; set; } = null!; // Description of the criterion
        public double MaxScore { get; set; } // Maximum score for the criterion
        [Range(0, double.MaxValue, ErrorMessage = "Score must be a non-negative number.")]
        public double Score { get; set; } // Score awarded for the criterion
        [StringLength(1000)]
        public string Remarks { get; set; } = null!; // Optional remarks or comments on the criterion

    }
}
