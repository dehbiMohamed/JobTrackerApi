public interface IAuthService
{
    Task<(bool ok, string? error, object? payload)> RegisterAsync(string email, string password);
    Task<(bool ok, string? error, object? payload)> LoginAsync(string email, string password);

    Task<(bool ok, string? error, object? payload)> ConfirmEmailAsync(Guid userId, string token);
}
