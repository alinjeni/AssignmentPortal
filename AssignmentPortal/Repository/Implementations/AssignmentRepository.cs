using AssignmentPortal.Models;
using System.Data;
using AssignmentPortal.Repository.Interfaces;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AssignmentPortal.Repository.Implementations
{
    public class AssignmentRepository: IAssignmentRepository
    {
        private readonly IDbConnection _db;

        public AssignmentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByClassroomAsync(int classroomId, bool includeArchived = false)
        {
            var sql = @"
                    SELECT * FROM Assignments 
                    WHERE ClassroomId = @ClassroomId
                    AND IsArchived = @IncludeArchived
                    ORDER BY DueDate DESC";
            return await _db.QueryAsync<Assignment>(sql, new { ClassroomId = classroomId, IncludeArchived = includeArchived });
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            var sql = "SELECT * FROM Assignments WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Assignment>(sql, new { Id = id });
        }

        public async Task<int> CreateAssignmentAsync(Assignment assignment)
        {
            var sql = @"INSERT INTO Assignments (Title, Description, DueDate, CreatedById, ClassroomId, CreatedAt)
                    VALUES (@Title, @Description, @DueDate, @CreatedById, @ClassroomId, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _db.ExecuteScalarAsync<int>(sql, assignment);
        }

        public async Task<bool> UpdateAssignmentAsync(Assignment assignment)
        {
            var sql = @"UPDATE Assignments 
                    SET Title = @Title, Description = @Description, DueDate = @DueDate,UpdatedAt =@UpdatedAt
                    WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, assignment);
            return result > 0;
        }
        public async Task<bool> AddFileAsync(AssignmentFile file)
        {
            var sql = @"INSERT INTO AssignmentFiles (AssignmentId, FileName, FilePath, FileSize, MimeType, UploadedAt, UploadedById)
                    VALUES (@AssignmentId, @FileName, @FilePath, @FileSize, @MimeType, @UploadedAt, @UploadedById)";
            var result = await _db.ExecuteAsync(sql, file);
            return result > 0;
        }

        public async Task<bool> AddCommentAsync(AssignmentComment comment)
        {
            var sql = @"INSERT INTO AssignmentComments (AssignmentId, UserId, Content, CreatedAt,UserName)
                    VALUES (@AssignmentId, @UserId, @Content, @CreatedAt, @UserName)";
            var result = await _db.ExecuteAsync(sql, comment);
            return result > 0;
        }

        public async Task<IEnumerable<AssignmentFile>> GetFilesByAssignmentIdAsync(int assignmentId)
        {
            var sql = "SELECT * FROM AssignmentFiles WHERE AssignmentId = @AssignmentId";
            return await _db.QueryAsync<AssignmentFile>(sql, new { AssignmentId = assignmentId });
        }

        public async Task<IEnumerable<AssignmentComment>> GetCommentsByAssignmentIdAsync(int assignmentId)
        {
            var sql = "SELECT * FROM AssignmentComments WHERE AssignmentId = @AssignmentId ORDER BY CreatedAt DESC";
            return await _db.QueryAsync<AssignmentComment>(sql, new { AssignmentId = assignmentId });
        }
        public async Task<bool> ArchiveAssignmentAsync(int id)
        {
            var sql = "UPDATE Assignments SET IsArchived = 1 WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
        public async Task<bool> UnArchiveAssignmentAsync(int id)
        {
            var sql = "UPDATE Assignments SET IsArchived = 0 WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
        public async Task<AssignmentFile?> GetFileByIdAsync(int fileId)
        {
            var sql = "SELECT * FROM AssignmentFiles WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<AssignmentFile>(sql, new { Id = fileId });
        }
        public async Task<bool> DeleteAssignmentFileAsync(int fileId)
        {
            var sql = "DELETE FROM AssignmentFiles WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new { Id = fileId });
            return result > 0;
        }
        public async Task<IEnumerable<StudentAssignmentViewModel>> GetAssignmentsForStudentAsync(int studentId)
        {
            var sql = @"
        SELECT a.Id AS AssignmentId, a.Title, a.DueDate, c.Name AS ClassroomName,
               CASE WHEN s.Id IS NOT NULL THEN 1 ELSE 0 END AS IsSubmitted,
               CASE WHEN s.TotalGrade IS NOT NULL THEN 1 ELSE 0 END AS IsGraded
        FROM Assignments a
        JOIN Classrooms c ON a.ClassroomId = c.Id
        JOIN ClassroomMembers cm ON cm.ClassroomId = c.Id
        LEFT JOIN Submissions s ON s.AssignmentId = a.Id AND s.UserId = @StudentId
        WHERE cm.StudentId = @StudentId and c.IsArchived = 0 and a.IsArchived=0";

            return await _db.QueryAsync<StudentAssignmentViewModel>(sql, new { StudentId = studentId });
        }
        public async Task<IEnumerable<FacultyAssignmentViewModel>> GetAssignmentsForFacultyAsync(int facultyId)
        {
            var sql = @"
        SELECT a.Id AS AssignmentId, a.Title, c.Name AS ClassroomName,
               COUNT(s.Id) AS SubmissionCount,
               AVG(CAST(s.TotalGrade AS FLOAT)) AS AverageGrade
        FROM Assignments a
        JOIN Classrooms c ON a.ClassroomId = c.Id
        LEFT JOIN Submissions s ON s.AssignmentId = a.Id
        WHERE c.CreatedById = @FacultyId and c.IsArchived = 0 and a.IsArchived=0
        GROUP BY a.Id, a.Title, c.Name";

            return await _db.QueryAsync<FacultyAssignmentViewModel>(sql, new { FacultyId = facultyId });
        }
    }
}
