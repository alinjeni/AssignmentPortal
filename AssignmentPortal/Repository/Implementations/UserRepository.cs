using AssignmentPortal.Models;
using System.Data;
using AssignmentPortal.Repository.Interfaces;
using AssignmentPortal.Services;
using Dapper;

namespace AssignmentPortal.Repository.Implementations
{
    public class UserRepository: IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            string sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM Users WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            string sql = "SELECT * FROM Users";
            return await _db.QueryAsync<User>(sql);
        }

        public async Task<int> CreateAsync(User user)
        {
            string sql = @"
            INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt)
            VALUES (@Name, @Email, @PasswordHash, @Role, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _db.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                return false;

            var hashedInput = PasswordHelper.HashPassword(password);
            return user.PasswordHash == hashedInput;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            var sql = "SELECT * FROM Users WHERE Role = @Role";
            return await _db.QueryAsync<User>(sql, new { Role = role });
        }

    }
}
