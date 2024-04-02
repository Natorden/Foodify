using Microsoft.AspNetCore.Identity;
namespace AuthService.Core.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string? DisplayName { get; set; }
    /// <summary>
    /// Gets or sets the path to the profile picture of the user.
    /// </summary>
    public string? ProfilePicturePath { get; set; }
    /// <summary>
    /// Represents the list of refresh tokens associated with a user.
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; }
}
