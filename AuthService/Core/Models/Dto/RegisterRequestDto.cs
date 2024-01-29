using System.ComponentModel.DataAnnotations;
namespace AuthService.Core.Models.Dto;

public class RegisterRequestDto
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}
