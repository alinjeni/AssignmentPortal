using AssignmentPortal.Models;
using System.Data;
using AssignmentPortal.Repository.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AssignmentPortal.Repository.Implementations
{
    public class ClassroomRepository: IClassroomRepository
    {
        private readonly IDbConnection _db;

        public ClassroomRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Classroom>> GetAllClassroomsByFacultyAsync(int facultyId, bool isArchived)
        {
            var sql = @"SELECT * FROM Classrooms 
                WHERE CreatedById = @FacultyId AND IsArchived = @IsArchived";
            return await _db.QueryAsync<Classroom>(sql, new { FacultyId = facultyId, IsArchived = isArchived });
        }

        public async Task<Classroom?> GetClassroomByIdAsync(int id)
        {
            var sql = "SELECT * FROM Classrooms WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Classroom>(sql, new { Id = id });
        }

        public async Task<int> CreateClassroomAsync(Classroom classroom)
        {
            var sql = @"INSERT INTO Classrooms (Name, Description, CreatedById, CreatedAt)
                    VALUES (@Name, @Description, @CreatedById, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _db.ExecuteScalarAsync<int>(sql, classroom);
        }

        public async Task<bool> AddStudentToClassroomAsync(int classroomId, int studentId)
        {
            var sql = @"INSERT INTO ClassroomMembers (ClassroomId, StudentId) 
                    VALUES (@ClassroomId, @StudentId)";
            var result = await _db.ExecuteAsync(sql, new { ClassroomId = classroomId, StudentId = studentId });
            return result > 0;
        }

        public async Task<IEnumerable<User>> GetStudentsInClassroomAsync(int classroomId)
        {
            var sql = @"
            SELECT u.* FROM Users u
            INNER JOIN ClassroomMembers cm ON cm.StudentId = u.Id
            WHERE cm.ClassroomId = @ClassroomId";
            return await _db.QueryAsync<User>(sql, new { ClassroomId = classroomId });
        }

        public async Task<bool> RemoveStudentFromClassroomAsync(int classroomId, int studentId)
        {
            var sql = @"DELETE FROM ClassroomMembers 
                    WHERE ClassroomId = @ClassroomId AND StudentId = @StudentId";
            var result = await _db.ExecuteAsync(sql, new { ClassroomId = classroomId, StudentId = studentId });
            return result > 0;
        }

        public async Task ArchiveClassroomAsync(int classroomId)
        {
            string sql = "UPDATE Classrooms SET IsArchived = 1 WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = classroomId });
        }
        public async Task UnArchiveClassroomAsync(int classroomId)
        {
            string sql = "UPDATE Classrooms SET IsArchived = 0 WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = classroomId });
        }
        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            var sql = @"UPDATE Classrooms 
                    SET Name = @Name, Description = @Description
                    WHERE Id = @Id";
            var result = await _db.ExecuteAsync(sql, classroom);
            return result > 0;
        }
        public async Task<IEnumerable<Classroom>> GetClassroomsForStudentAsync(int studentId, bool isArchived)
        {
            var sql = @"
        SELECT c.*,u.Name as CreatedByName FROM Classrooms c
        INNER JOIN ClassroomMembers cm ON cm.ClassroomId = c.Id
        INNER JOIN Users u ON c.CreatedById = u.Id
        WHERE cm.StudentId = @StudentId AND c.IsArchived = @IsArchived";
            return await _db.QueryAsync<Classroom>(sql, new { StudentId = studentId, IsArchived = isArchived });
        }
    }
}
