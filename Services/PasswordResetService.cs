using System.Security.Cryptography;
using System.Text;
using JobTracker.api.Interfaces;

namespace JobTracker.api.Services;

public class PasswordResetService(
    IUserService users,
    IEmailSender emailSender,
    IHttpContextAccessor http,
    IConfiguration config
) : IPasswordResetService
{
    private readonly IConfiguration _config = config;


    public async Task RequestResetAsync(string email)
    {

        var user = await users.GetByEmailAsync(email);

        
        if (user is null) return;

        var token = NewToken();

        user.PasswordResetTokenHash = Sha256(token);
        user.PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddHours(1);

        await users.SaveChangesAsync();

        var baseUrl = _config["App:FrontendBaseUrl"] ?? "http://localhost:4200";
        var resetUrl = $"{baseUrl}/reset-password?userId={user.Id}&token={token}";



        await emailSender.SendAsync(
            user.Email,
            "Réinitialisation du mot de passe",
            $"""
            <h3>Mot de passe oublié</h3>
            <p>Clique ici pour réinitialiser ton mot de passe :</p>
            <p><a href="{resetUrl}">Réinitialiser mon mot de passe</a></p>
            <p>Ce lien expire dans 1 heure.</p>
            """
        );
    }

    public async Task<(bool ok, string? error)> ResetAsync(Guid userId, string token, string newPassword)
    {
        var user = await users.GetByIdAsync(userId);
        if (user is null) return (false, "Lien invalide.");

        if (user.PasswordResetTokenExpiresAtUtc is null || user.PasswordResetTokenExpiresAtUtc < DateTime.UtcNow)
            return (false, "Lien expiré.");

        var hash = Sha256(token);
        if (user.PasswordResetTokenHash != hash)
            return (false, "Lien invalide.");

      
        users.SetPassword(user, newPassword);

   
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAtUtc = null;

        await users.SaveChangesAsync();
        return (true, null);
    }

    private string GetBaseUrl()
    {
        var ctx = http.HttpContext;
        if (ctx is null) return "http://localhost:5221";
        return $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    }

    private static string NewToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(bytes);
        return token.Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string Sha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
