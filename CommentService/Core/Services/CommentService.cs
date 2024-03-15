using CommentService.Core.Interfaces;
using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Entities;
using CommentService.Infrastructure.Interfaces;
namespace CommentService.Core.Services;

public class CommentService : ICommentService{
    private readonly ICommentRepository _commentRepository;
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    public async Task<List<Comment>> GetCommentsByRecipeId(Guid recipeId)
    {
        return await _commentRepository.GetCommentsByRecipeId(recipeId);
    }
    
    public async Task<Comment?> GetCommentById(Guid commentId)
    {
        return await _commentRepository.GetCommentById(commentId);
    }
    
    public async Task<Comment> CreateComment(CommentPostBindingModel model)
    {
        var commentId = await _commentRepository.CreateComment(model);
        return (await _commentRepository.GetCommentById(commentId))!;
    }
}
