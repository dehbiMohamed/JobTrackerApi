using System.Security.Cryptography;
using System.Text;
using JobTracker.api.Data;
using JobTracker.api.Interfaces;
using Microsoft.Extensions.Configuration;

namespace JobTracker.api.Services;

public class EmailConfirmationService(
    IUserService users,
    IEmailSender emailSender,
    IConfiguration config
) : IEmailConfirmationService
{
    public async Task SendConfirmationAsync(User user)
    {
        var token = NewToken();

        user.EmailConfirmed = false;
        user.EmailConfirmTokenHash = Sha256(token);
        user.EmailConfirmTokenExpiresAtUtc = DateTime.UtcNow.AddHours(24);

        await users.SaveChangesAsync();

        var frontendBaseUrl = GetFrontendBaseUrl();
        var confirmUrl =
            $"{frontendBaseUrl}/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await emailSender.SendAsync(
            user.Email,
            "Confirme ton email",
            $"""
            <h3>Bienvenue sur JobTracker</h3>
            <p>Clique ici pour confirmer ton email :</p>
            <p><a href="{confirmUrl}">Confirmer mon email</a></p>
            <p>Ce lien expire dans 24h.</p>
            """
        );
    }

    public async Task<(bool ok, string? error)> ConfirmAsync(Guid userId, string token)
    {
        var user = await users.GetByIdAsync(userId);
        if (user is null) return (false, "Lien invalide.");

        if (user.EmailConfirmed) return (true, null);

        if (user.EmailConfirmTokenExpiresAtUtc is null || user.EmailConfirmTokenExpiresAtUtc < DateTime.UtcNow)
            return (false, "Lien expiré.");

        var hash = Sha256(token);
        if (user.EmailConfirmTokenHash != hash)
            return (false, "Lien invalide.");

        user.EmailConfirmed = true;
        user.EmailConfirmTokenHash = null;
        user.EmailConfirmTokenExpiresAtUtc = null;

        await users.SaveChangesAsync();
        return (true, null);
    }

    private string GetFrontendBaseUrl()
    {
        var url = config["App:FrontendBaseUrl"];
        if (string.IsNullOrWhiteSpace(url)) return "http://localhost:4200"; // fallback dev
        return url.TrimEnd('/');
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
