using JobTracker.api.Data;

namespace JobTracker.api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);

    }
}
