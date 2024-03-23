using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Entities;
namespace CommentService.Infrastructure.Interfaces;

/// <summary>
/// Represents the interface for a comment repository.
/// </summary>
public interface ICommentRepository {
    
    /// <summary>
    /// Retrieves all comments for a recipe.
    /// </summary>
    /// <param name="recipeId">The unique identifier of the recipe.</param>
    /// <returns>A list of comments for the specified recipe.</returns>
    Task<List<Comment>> GetCommentsByRecipeId(Guid recipeId);
    
    /// <summary>
    /// Retrieves a comment by its unique identifier.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment.</param>
    /// <returns>The comment entity with the specified commentId.</returns>
    Task<Comment?> GetCommentById(Guid commentId);

    /// <summary>
    /// Retrieves the count of comments for a recipe.
    /// </summary>
    /// <param name="recipeId">The unique identifier of the recipe.</param>
    /// <returns>The count of comments for the specified recipe.</returns>
    Task<int> GetCommentCount(Guid recipeId);

    /// <summary>
    /// Retrieves the comment counts for multiple recipes.
    /// </summary>
    /// <param name="recipeIds">The unique identifiers of the recipes.</param>
    /// <returns>A list of tuples containing the recipe ID and its corresponding comment count.</returns>
    Task<List<(Guid id, int count)>> getCommentCountsByRecipeIds(IEnumerable<Guid> recipeIds);

    /// <summary>
    /// Create a comment about a recipe.
    /// </summary>
    /// <param name="model">The binding model for creating a comment.</param>
    /// <returns>The unique identifier of the created comment.</returns>
    Task<Guid> CreateComment(CommentPostBindingModel model);

}
