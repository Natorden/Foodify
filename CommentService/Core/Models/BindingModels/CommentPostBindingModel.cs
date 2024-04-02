namespace CommentService.Core.Models.BindingModels;

/// <summary>
/// Represents the binding model for creating a comment about a recipe.
/// </summary>
public class CommentPostBindingModel {
    /// <summary>
    /// Represents the unique identifier for a recipe.
    /// </summary>
    public Guid RecipeId { get; set; }
    /// <summary>
    /// Represents the content of a comment.
    /// </summary>
    public required string Content { get; set; }
}
