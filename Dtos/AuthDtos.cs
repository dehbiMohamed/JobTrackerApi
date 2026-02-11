using System.ComponentModel.DataAnnotations;

namespace JobTracker.api.Dtos;

public record RegisterDto(
    [param: Required]
    [param: EmailAddress]
    string Email,
    [param: Required]
    [param: MinLength(6)]
    string Password
);

public record LoginDto(
    [param: Required]
    [param: EmailAddress]
    string Email,
    [param: Required]
    string Password
);
