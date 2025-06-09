using AssignmentPortal.Models;
using AssignmentPortal.Services;
using Dapper;
using System.Data;

namespace AssignmentPortal.Data
{
    public class DatabaseSeeder
    {
        private readonly IDbConnection _db;

        public DatabaseSeeder(IDbConnection db)
        {
            _db = db;
        }

        public async Task SeedUsersAsync()
        {
            var usersToSeed = new[]
            {
                new { Name = "Student One", Email = "student1@example.com", Password = "student123", Role = "Student" },
                new { Name = "Faculty One", Email = "faculty1@example.com", Password = "faculty123", Role = "Faculty" },
                new { Name = "Student Two", Email = "student2@example.com", Password = "student234", Role = "Student" },
                new { Name = "Faculty Two", Email = "faculty2@example.com", Password = "faculty234", Role = "Faculty" }
            };

            var emails = usersToSeed.Select(u => u.Email).ToArray();

            var existingUsers = (await _db.QueryAsync<string>(
                "SELECT Email FROM Users WHERE Email IN @emails", new { emails }))
                .ToHashSet();

            var newUsers = usersToSeed
                .Where(u => !existingUsers.Contains(u.Email))
                .Select(u => new
                {
                    u.Name,
                    u.Email,
                    PasswordHash = PasswordHelper.HashPassword(u.Password),
                    u.Role
                });

            if (newUsers.Any())
            {
                var sql = @"
                    INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt)
                    VALUES (@Name, @Email, @PasswordHash, @Role, GETUTCDATE())";

                await _db.ExecuteAsync(sql, newUsers);
            }
        }
    }
}
