using JobTracker.api.Config;
using JobTracker.api.Data;
using JobTracker.api.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobTracker.api.Services;

public class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public string CreateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwt.ExpiresDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
