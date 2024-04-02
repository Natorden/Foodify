using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Entities;
namespace CommentService.Core.Interfaces;

/// <summary>
/// Represents an interface for a service for managing comments on recipes.
/// </summary>
public interface ICommentService {
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
    /// Create a comment about a recipe.
    /// </summary>
    /// <param name="model">The binding model for creating a comment.</param>
    /// <returns>The created comment.</returns>
    Task<Comment> CreateComment(CommentPostBindingModel model);
    
    
}
