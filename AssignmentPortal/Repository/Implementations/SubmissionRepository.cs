using AssignmentPortal.Models;
using System.Data;
using AssignmentPortal.Repository.Interfaces;
using Dapper;

namespace AssignmentPortal.Repository.Implementations
{
    public class SubmissionRepository: ISubmissionRepository
    {
        private readonly IDbConnection _db;

        public SubmissionRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> CreateSubmissionAsync(Submission submission)
        {
            var sql = @"INSERT INTO Submissions (AssignmentId, UserId, Content, SubmittedAt)
                    VALUES (@AssignmentId, @UserId, @Content, @SubmittedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _db.ExecuteScalarAsync<int>(sql, submission);
        }

        public async Task<Submission?> GetSubmissionByIdAsync(int id)
        {
            var sql = @"SELECT * FROM Submissions WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Submission>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            var sql = @"SELECT * FROM Submissions WHERE AssignmentId = @AssignmentId";
            return await _db.QueryAsync<Submission>(sql, new { AssignmentId = assignmentId });
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByUserIdAsync(int userId)
        {
            var sql = @"SELECT * FROM Submissions WHERE UserId = @UserId";
            return await _db.QueryAsync<Submission>(sql, new { UserId = userId });
        }

        public async Task<bool> UpdateSubmissionAsync(Submission submission)
        {
            var sql = @"UPDATE Submissions
                    SET Content = @Content, SubmittedAt = @SubmittedAt,
                    GradedAt= @GradedAt,
                    Feedback = @Feedback, TotalGrade=@TotalGrade
                    WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, submission);
            return result > 0;
        }

        public async Task<bool> GradeSubmissionAsync(int submissionId, double totalGrade, string feedback)
        {
            var sql = @"UPDATE Submissions
                    SET TotalGrade = @TotalGrade, Feedback = @Feedback, GradedAt = @GradedAt
                    WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new
            {
                Id = submissionId,
                TotalGrade = totalGrade,
                Feedback = feedback,
                GradedAt = DateTime.UtcNow
            });
            return result > 0;
        }

        public async Task<bool> DeleteSubmissionAsync(int id)
        {
            var sql = @"DELETE FROM Submissions WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
        public async Task<IEnumerable<SubmissionFile>> GetFilesBySubmissionIdAsync(int submissionId)
        {
            var sql = "SELECT * FROM SubmissionFiles WHERE SubmissionId = @SubmissionId";
            return await _db.QueryAsync<SubmissionFile>(sql, new { SubmissionId = submissionId });
        }
        public async Task AddSubmissionFileAsync(SubmissionFile file)
        {
            var sql = @"INSERT INTO SubmissionFiles (SubmissionId, FileName, FilePath, FileSize, MimeType, UploadedAt, UploadedById)
                VALUES (@SubmissionId, @FileName, @FilePath, @FileSize, @MimeType, @UploadedAt, @UploadedById)";
            await _db.ExecuteAsync(sql, file);
        }
        public async Task<Submission?> GetSubmissionByUserAndAssignmentAsync(int userId, int assignmentId)
        {
            var sql = @"SELECT * FROM Submissions 
                WHERE UserId = @UserId AND AssignmentId = @AssignmentId";

            return await _db.QueryFirstOrDefaultAsync<Submission>(sql, new
            {
                UserId = userId,
                AssignmentId = assignmentId
            });
        }
        public async Task<bool> DeleteSubmissionFileAsync(int fileId)
        {
            var sql = "DELETE FROM SubmissionFiles WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, new { Id = fileId });
            return result > 0;
        }
        public async Task<SubmissionFile?> GetFileByIdAsync(int fileId)
        {
            var sql = "SELECT * FROM SubmissionFiles WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<SubmissionFile>(sql, new { Id = fileId });
        }
        public async Task<IEnumerable<StudentSubmissionViewModel>> GetSubmissionsForStudentAsync(int studentId)
        {
            var sql = @"
        SELECT a.Id AS AssignmentId, a.Title AS AssignmentTitle, s.SubmittedAt, s.TotalGrade, s.Feedback
        FROM Submissions s
        JOIN Assignments a ON a.Id = s.AssignmentId
        JOIN Classrooms c ON c.Id = a.ClassroomId
        WHERE s.UserId = @StudentId and c.IsArchived =0 and a.IsArchived=0";

            return await _db.QueryAsync<StudentSubmissionViewModel>(sql, new { StudentId = studentId });
        }
        public async Task<IEnumerable<FacultySubmissionViewModel>> GetSubmissionsForFacultyAsync(int facultyId)
        {
            var sql = @"
        SELECT s.Id AS SubmissionId, u.Name AS StudentName, a.Title AS AssignmentTitle,
               s.SubmittedAt, s.TotalGrade
        FROM Submissions s
        JOIN Assignments a ON a.Id = s.AssignmentId
        JOIN Classrooms c ON c.Id = a.ClassroomId
        JOIN Users u ON u.Id = s.UserId
        WHERE c.CreatedById = @FacultyId and c.IsArchived =0 and a.IsArchived=0";

            return await _db.QueryAsync<FacultySubmissionViewModel>(sql, new { FacultyId = facultyId });
        }
    }
}
