using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface IAssessmentRepository
    {
        Task<IEnumerable<AssessmentCriterion>> GetAllCriteriaAsync();
        Task<int> CreateAssessmentAsync(Assessment assessment);
        Task<bool> AddEvaluatedCriteriaAsync(List<AssessedCriterion> assessedCriteria);
        Task<IEnumerable<AssessedCriterion>> GetEvaluatedCriteriaBySubmissionIdAsync(int submissionId);
        Task<Assessment?> GetAssessmentBySubmissionIdAsync(int submissionId);
        Task<Assessment?> GetBySubmissionIdAsync(int submissionId);
        Task<List<AssessmentResultViewModel>> GetCriteriaResultsAsync(int assessmentId);
    }
}
