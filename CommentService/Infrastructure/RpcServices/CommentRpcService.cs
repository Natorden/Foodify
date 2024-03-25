using CommentService.Infrastructure.Interfaces;
using Grpc.Core;
using Shared;
namespace CommentService.Infrastructure.RpcServices;

public class CommentRpcService : Comment.CommentBase {
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentRpcService> _logger;
    
    public CommentRpcService(ICommentRepository commentRepository, ILogger<CommentRpcService> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async override Task<CommentCountReturnModel> GetRecipeCommentCount(CommentCountLookupModel request, ServerCallContext context)
    {
        var commentCount = await _commentRepository.GetCommentCount(Guid.Parse(request.RecipeId));
        var commentCountReturnModel = new CommentCountReturnModel {
            Id = request.RecipeId,
            Count = commentCount
        };
        return commentCountReturnModel;
    }

    public async override Task<CommentCountMultiReturnModel> GetManyRecipeCommentCounts(CommentCountMultiLookupModel request, ServerCallContext context)
    {
        var commentCountMultiReturnModel = new CommentCountMultiReturnModel();
        var commentCounts = await _commentRepository.getCommentCountsByRecipeIds(request.RecipeIds.Select(Guid.Parse));
        var commentCountReturnModels = commentCounts.Select(tp =>
            new CommentCountReturnModel {
                Id = tp.id.ToString(),
                Count = tp.count
            }
        );
        commentCountMultiReturnModel.Comments.AddRange(commentCountReturnModels);
        return commentCountMultiReturnModel;
    }
}
