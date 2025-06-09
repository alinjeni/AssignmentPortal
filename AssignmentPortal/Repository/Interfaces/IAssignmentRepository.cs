using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAssignmentsByClassroomAsync(int classroomId, bool includeArchived);
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<int> CreateAssignmentAsync(Assignment assignment);
        Task<bool> UpdateAssignmentAsync(Assignment assignment);
        Task<bool> AddFileAsync(AssignmentFile file);
        Task<bool> AddCommentAsync(AssignmentComment comment);
        Task<IEnumerable<AssignmentFile>> GetFilesByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<AssignmentComment>> GetCommentsByAssignmentIdAsync(int assignmentId);
        Task<bool> ArchiveAssignmentAsync(int assignmentId);
        Task<bool> UnArchiveAssignmentAsync(int assignmentId);
        Task<AssignmentFile?> GetFileByIdAsync(int fileId);
        Task<bool> DeleteAssignmentFileAsync(int fileId);
        Task<IEnumerable<StudentAssignmentViewModel>> GetAssignmentsForStudentAsync(int studentId);
        Task<IEnumerable<FacultyAssignmentViewModel>> GetAssignmentsForFacultyAsync(int facultyId);
    }
}
