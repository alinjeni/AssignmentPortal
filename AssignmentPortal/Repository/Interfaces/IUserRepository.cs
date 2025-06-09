using AssignmentPortal.Models;

namespace AssignmentPortal.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> CreateAsync(User user);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    }
}
