using JobTracker.api.Data;
using JobTracker.api.Dtos;
using JobTracker.api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (ok, error, payload) = await auth.RegisterAsync(dto.Email, dto.Password);

        if (!ok) return BadRequest(error);

        return Created("api/auth/login", payload);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (ok, error, payload) = await auth.LoginAsync(dto.Email, dto.Password);
        if (!ok) return Unauthorized(error);

        return Ok(payload);
    }
    [HttpPost("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> TestEmail([FromServices] IEmailSender email)
    {
        await email.SendAsync("test@mail.com", "Test Mailtrap", "<p>SMTP OK ✅</p>");
        return Ok("Email envoyé (check Mailtrap)");
    }
    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var (ok, error, payload) = await auth.ConfirmEmailAsync(userId, token);
        if (!ok) return BadRequest(error);
        return Ok(payload);
    }
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordDto dto,
    [FromServices] IPasswordResetService reset
)
    {
        await reset.RequestResetAsync(dto.Email);

        return Ok(new { message = "Si cet email existe, un lien a été envoyé." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto,
        [FromServices] IPasswordResetService reset
    )
    {
        var (ok, error) = await reset.ResetAsync(dto.UserId, dto.Token, dto.NewPassword);
        if (!ok) return BadRequest(error);

        return Ok(new { message = "Mot de passe mis à jour ✅" });
    }

}
