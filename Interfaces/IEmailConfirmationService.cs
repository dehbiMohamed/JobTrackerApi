using JobTracker.api.Data;

namespace JobTracker.api.Interfaces;

public interface IEmailConfirmationService
{
    Task SendConfirmationAsync(User user);
    Task<(bool ok, string? error)> ConfirmAsync(Guid userId, string token);
}
