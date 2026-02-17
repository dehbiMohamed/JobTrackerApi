namespace JobTracker.api.Interfaces;

public interface IPasswordResetService
{
    Task RequestResetAsync(string email); 
    Task<(bool ok, string? error)> ResetAsync(Guid userId, string token, string newPassword);
}
