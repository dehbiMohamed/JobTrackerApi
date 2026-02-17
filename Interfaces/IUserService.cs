using JobTracker.api.Data;

namespace JobTracker.api.Interfaces;

public interface IUserService
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
    Task<User> CreateUserAsync(string email, string password);
    Task<User?> GetByIdAsync(Guid id);
    void SetPassword(User user, string newPassword);

    Task SaveChangesAsync();

}
