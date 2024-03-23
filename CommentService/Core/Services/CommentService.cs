using CommentService.Core.Interfaces;
using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Entities;
using CommentService.Infrastructure.Interfaces;
namespace CommentService.Core.Services;

public class CommentService : ICommentService{
    private readonly ICommentRepository _commentRepository;
    private readonly IProfileRpcClient _profileRpcClient;
    public CommentService(ICommentRepository commentRepository, IProfileRpcClient profileRpcClient)
    {
        _commentRepository = commentRepository;
        _profileRpcClient = profileRpcClient;
    }
    
    public async Task<List<Comment>> GetCommentsByRecipeId(Guid recipeId)
    {
        return await _commentRepository.GetCommentsByRecipeId(recipeId);
    }
    
    public async Task<Comment?> GetCommentById(Guid commentId)
    {
        var comment = await _commentRepository.GetCommentById(commentId);
        if (comment is null) {
            return null;
        }
        comment.User = await _profileRpcClient.GetUserProfileById(comment.UserId);
        return comment;
    }
    
    public async Task<Comment> CreateComment(CommentPostBindingModel model)
    {
        var commentId = await _commentRepository.CreateComment(model);
        return (await _commentRepository.GetCommentById(commentId))!;
    }
}
