using System.ComponentModel.DataAnnotations;

namespace JobTracker.api.Dtos;

public record ForgotPasswordDto(
    [param: Required]
    [param: EmailAddress]
    string Email
);
