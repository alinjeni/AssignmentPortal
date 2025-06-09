using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<StudentDashboardViewModel> GetStudentDashboardDataAsync(int studentId);
        Task<FacultyDashboardViewModel> GetFacultyDashboardDataAsync(int facultyId);
    }
}
