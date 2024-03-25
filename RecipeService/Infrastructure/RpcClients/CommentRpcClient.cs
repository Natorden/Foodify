using Grpc.Core;
using RecipeService.Infrastructure.Interfaces;
using Shared;
namespace RecipeService.Infrastructure.RpcClients;

public class CommentRpcClient : ICommentRpcClient {
    private readonly Comment.CommentClient _commentClient;
    public CommentRpcClient(Comment.CommentClient commentClient)
    {
        _commentClient = commentClient;
    }

    public async Task<int?> GetCommentCountByRecipeId(Guid recipeId)
    {
        try {
            var response = await _commentClient.GetRecipeCommentCountAsync(
                new CommentCountLookupModel{
                    RecipeId = recipeId.ToString()
                }
            );
            return response.Count;
        } catch (RpcException) {
            Console.WriteLine("Comment service unavailable");
            return null;
        }
    }
    
    public async Task<List<(Guid recipeId, int count)>> GetCommentCountsByRecipeIds(IEnumerable<Guid> ids)
    {
        var request = new CommentCountMultiLookupModel();
        request.RecipeIds.AddRange(ids.Select(id => id.ToString()));
        
        try {
            var response = await _commentClient.GetManyRecipeCommentCountsAsync(request);
            return response.Comments.Select(result => (Guid.Parse(result.Id), result.Count)).ToList();
        } catch (RpcException) {
            Console.WriteLine("Comment service unavailable");
            return [];
        }
    }
}
