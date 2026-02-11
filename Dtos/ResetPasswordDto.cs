using System.ComponentModel.DataAnnotations;

namespace JobTracker.api.Dtos;

public record ResetPasswordDto(
    [param: Required]
    Guid UserId,

    [param: Required]
    string Token,

    [param: Required]
    [param: MinLength(6)]
    string NewPassword
);
