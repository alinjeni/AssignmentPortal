using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<int> CreateSubmissionAsync(Submission submission);
        Task<Submission?> GetSubmissionByIdAsync(int id);
        Task<IEnumerable<Submission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<Submission>> GetSubmissionsByUserIdAsync(int userId);
        Task<bool> UpdateSubmissionAsync(Submission submission);
        Task<bool> GradeSubmissionAsync(int submissionId, double totalGrade, string feedback);
        Task<bool> DeleteSubmissionAsync(int id);
        Task<IEnumerable<SubmissionFile>> GetFilesBySubmissionIdAsync(int submissionId);
        Task AddSubmissionFileAsync(SubmissionFile file);
        Task<Submission?> GetSubmissionByUserAndAssignmentAsync(int userId, int assignmentId);
        Task<bool> DeleteSubmissionFileAsync(int fileId);
        Task<SubmissionFile?> GetFileByIdAsync(int fileId);
        Task<IEnumerable<StudentSubmissionViewModel>> GetSubmissionsForStudentAsync(int studentId);
        Task<IEnumerable<FacultySubmissionViewModel>> GetSubmissionsForFacultyAsync(int facultyId);
    }
}
