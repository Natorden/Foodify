using System.ComponentModel.DataAnnotations;
namespace AuthService.Core.Models.Dto;

public class LoginRequestDto
{
    /// <summary>
    /// Represents the email or username property of a login request model.
    /// </summary>
    public required string EmailOrUserName { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    /// <example>P@ssw0rd.+</example>
    public required string Password { get; init; }
}
