using JobTracker.api.Data;
using JobTracker.api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.api.Services;

public class UserService(AppDbContext db) : IUserService
{
    private readonly PasswordHasher<User> _hasher = new();

    public Task<bool> EmailExistsAsync(string email)
        => db.Users.AnyAsync(x => x.Email == NormalizeEmail(email));

    public Task<User?> GetByEmailAsync(string email)
        => db.Users.FirstOrDefaultAsync(x => x.Email == NormalizeEmail(email));

    public string HashPassword(User user, string password)
        => _hasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string password)
        => _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;

    public async Task<User> CreateUserAsync(string email, string password)
    {
        var normalized = NormalizeEmail(email);

        var user = new User
        {
            Email = normalized,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = HashPassword(user, password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();

    public Task<User?> GetByIdAsync(Guid id)
    => db.Users.FirstOrDefaultAsync(x => x.Id == id);
    public void SetPassword(User user, string newPassword)
    {
        user.PasswordHash = _hasher.HashPassword(user, newPassword);
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();

}
