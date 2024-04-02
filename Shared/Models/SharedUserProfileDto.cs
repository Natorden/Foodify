namespace Shared.Models;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a user profile.
/// </summary>
/// <remarks>
/// The purpose of this DTO is to see users without disclosing any private user data
/// </remarks>
public class SharedUserProfileDto {
    /// <summary>
    /// Represents the Id of the user profile.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the display name of a user profile.
    /// </summary>
    /// <remarks>
    /// This property represents the display name of a user, which is a name that is typically shown to other users.
    /// The display name can be any string value and may or may not be unique.
    /// </remarks>
    public string? DisplayName { get; set; }
    /// <summary>
    /// Gets or sets the username of the user profile.
    /// </summary>
    /// <remarks>
    /// This property represents the unique username of the user profile.
    /// </remarks>
    public required string UserName { get; set; }
    /// <summary>
    /// Gets or sets the profile picture path of a user.
    /// </summary>
    /// <remarks>
    /// The profile picture path is the location or path of the user's profile picture.
    /// This property is nullable, which means a user may not have a profile picture.
    /// </remarks>
    public string? ProfilePicturePath { get; set; }
}
