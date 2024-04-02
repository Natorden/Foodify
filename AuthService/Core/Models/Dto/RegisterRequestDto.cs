using System.ComponentModel.DataAnnotations;
namespace AuthService.Core.Models.Dto;

/// <summary>
/// Data transfer object for registering a new user.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Represents the user's email address
    /// </summary>
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Represents the username of a user. (user handle)
    /// </summary>
    /// <remarks>
    /// The username is a unique identifier that users use to log into the system.
    /// It must be between 3 and 32 characters long.
    /// </remarks>
    [Length(3, 32)]
    public required string UserName { get; init; }

    /// <summary>
    /// Represents the user's password.
    /// </summary>
    public required string Password { get; init; }
}
