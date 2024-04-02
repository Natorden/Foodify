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
        var comments = await _commentRepository.GetCommentsByRecipeId(recipeId);
        
        var userIds = comments.Select(r => r.UserId);
        var users = await _profileRpcClient.GetUserProfilesByIds(userIds);
        // Map the user profiles that were found
        comments.ForEach(comment =>
        {
            comment.User = users.FirstOrDefault(u => u.Id == comment.UserId);
        });

        return comments;
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
