namespace AuthService.Core.Models.BindingModels;

/// <summary>
/// Represents a binding model for updating a user profile.
/// </summary>
public class ProfilePutBindingModel {
    /// <summary>
    /// Represents the display name of the user.
    /// </summary>
    public string? DisplayName { get; set; }
    /// <summary>
    /// Represents the path to the profile picture of a user.
    /// </summary>
    public string? ProfilePicturePath { get; set; }
}
