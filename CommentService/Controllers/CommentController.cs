using CommentService.Core.Interfaces;
using CommentService.Core.Models.BindingModels;
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
    
    [HttpGet("recipe/{recipeId:guid}")]
    public async Task<IActionResult> GetCommentsByRecipeId(Guid recipeId)
    {
        var comments = await _commentService.GetCommentsByRecipeId(recipeId);
        return Ok(comments);
    }
    
    [HttpGet("{commentId:guid}")]
    public async Task<IActionResult> GetCommentById(Guid commentId)
    {
        var comment = await _commentService.GetCommentById(commentId);
        if (comment is null) {
            return NotFound();
        }
        return Ok(comment);
    }
    
    #endregion

    #region POST
    
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
