using JobTracker.api.Interfaces;

namespace JobTracker.api.Services;

public class AuthService(
    IUserService users,
    ITokenService tokens,
    IEmailConfirmationService emailConfirm
) : IAuthService
{
    public async Task<(bool ok, string? error, object? payload)> RegisterAsync(string email, string password)
    {
        if (await users.EmailExistsAsync(email))
            return (false, "Email déjà utilisé.", null);

        var user = await users.CreateUserAsync(email, password);

        await emailConfirm.SendConfirmationAsync(user);

        return (true, null, new { user.Id, user.Email });
    }

    public async Task<(bool ok, string? error, object? payload)> LoginAsync(string email, string password)
    {
        var user = await users.GetByEmailAsync(email);
        if (user is null) return (false, "Identifiants invalides.", null);

        if (!user.EmailConfirmed) return (false, "Email non confirmé.", null);

        var ok = users.VerifyPassword(user, password);
        if (!ok) return (false, "Identifiants invalides.", null);

        var token = tokens.CreateToken(user);
        return (true, null, new { token });
    }

    public async Task<(bool ok, string? error, object? payload)> ConfirmEmailAsync(Guid userId, string token)
    {
        var (ok, error) = await emailConfirm.ConfirmAsync(userId, token);
        if (!ok) return (false, error, null);
        return (true, null, new { message = "Email confirmé ✅" });
    }
}
