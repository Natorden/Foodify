using Shared.Models;
namespace CommentService.Infrastructure.Interfaces;

public interface IProfileRpcClient {
    /// <summary>
    /// Retrieves a user profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user profile.</param>
    /// <returns>The user profile with the specified identifier, or null if it doesn't exist.</returns>
    Task<SharedUserProfileDto?> GetUserProfileById(Guid id);
    /// <summary>
    /// Retrieves a list of user profiles by their unique identifiers.
    /// </summary>
    /// <param name="ids">The list of unique identifiers of the user profiles.</param>
    /// <returns>Returns a list of user profiles with the specified identifiers, or null if none of the profiles don't exist.</returns>
    Task<List<SharedUserProfileDto>?> GetUserProfilesByIds(List<Guid> ids);
}
