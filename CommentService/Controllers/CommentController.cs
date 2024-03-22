using CommentService.Core.Interfaces;
using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CommentService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase {
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    #region GET
    /// <summary>
    /// Retrieves all comments for a recipe.
    /// </summary>
    /// <param name="recipeId">The unique identifier of the recipe.</param>
    /// <returns>A list of comments for the specified recipe.</returns>
    [HttpGet("Recipe/{recipeId:guid}")]
    public async Task<IActionResult> GetCommentsByRecipeId(Guid recipeId)
    {
        var comments = await _commentService.GetCommentsByRecipeId(recipeId);
        return Ok(comments);
    }

    /// <summary>
    /// Retrieves a comment by its unique identifier.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment.</param>
    /// <returns>The comment entity with the specified commentId.</returns>
    /// <exception cref="NotFoundException">Thrown when the comment with the specified ID is not found.</exception>
    [HttpGet("{commentId:guid}")]
    public async Task<IActionResult> GetCommentById(Guid commentId)
    {
        var comment = await _commentService.GetCommentById(commentId);
        if (comment is null) {
            throw new NotFoundException("Comment with the specified ID not found");
        }
        return Ok(comment);
    }
    
    #endregion

    #region POST
    /// <summary>
    /// Creates a comment about a recipe.
    /// </summary>
    /// <param name="model">The binding model for creating a comment.</param>
    /// <returns>The created comment.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateComment(CommentPostBindingModel model)
    {
        var comment = await _commentService.CreateComment(model);
        
        return CreatedAtAction(nameof(GetCommentById), new {
            commentId = comment.Id
        }, comment);
    }
    
    #endregion
}
