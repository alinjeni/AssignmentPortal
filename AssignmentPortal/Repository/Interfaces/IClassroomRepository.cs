using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetAllClassroomsByFacultyAsync(int facultyId, bool isArchived);
        Task<Classroom?> GetClassroomByIdAsync(int id);
        Task<int> CreateClassroomAsync(Classroom classroom);
        Task<bool> AddStudentToClassroomAsync(int classroomId, int studentId);
        Task<IEnumerable<User>> GetStudentsInClassroomAsync(int classroomId);
        Task<bool> RemoveStudentFromClassroomAsync(int classroomId, int studentId);
        Task ArchiveClassroomAsync(int classroomId);
        Task UnArchiveClassroomAsync(int classroomId);
        Task<bool> UpdateClassroomAsync(Classroom classroom);
        Task<IEnumerable<Classroom>> GetClassroomsForStudentAsync(int studentId, bool isArchived);
    }
}
