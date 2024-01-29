using System.ComponentModel.DataAnnotations;
namespace AuthService.Core.Models.Dto;

public class LoginRequestDto
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    /// <example>user@app</example>
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    /// <example>P@ssw0rd.+</example>
    public required string Password { get; init; }
}
