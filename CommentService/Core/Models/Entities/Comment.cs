namespace CommentService.Core.Models.Entities;

/// <summary>
/// Represents a comment on a recipe made by a user.
/// </summary>
public class Comment {
    /// <summary>
    /// Represents the unique identifier of a comment.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Represents the unique identifier of a user who wrote the comment.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Represents the identifier of a recipe.
    /// </summary>
    public Guid RecipeId { get; set; }
    /// <summary>
    /// Represents the content of the comment.
    /// </summary>
    public required string Content { get; set; }
    /// <summary>
    /// Represents the date and time when the comment was created.
    /// </summary>
    /// <remarks>
    /// This property is set automatically when the comment is created
    /// and cannot be modified.
    /// </remarks>
    public DateTimeOffset CreatedAt { get; set; }
}
