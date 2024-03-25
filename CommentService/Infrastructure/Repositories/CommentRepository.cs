using CommentService.Core.Context;
using CommentService.Core.Models.BindingModels;
using CommentService.Core.Models.Entities;
using CommentService.Infrastructure.Factories;
using CommentService.Infrastructure.Interfaces;
using Dapper;
namespace CommentService.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly CurrentContext _currentContext;
    public CommentRepository(IDbConnectionFactory connectionFactory, CurrentContext currentContext)
    {
        _connectionFactory = connectionFactory;
        _currentContext = currentContext;
    }

    #region SELECT
    
    public async Task<List<Comment>> GetCommentsByRecipeId(Guid recipeId)
    {
        using var connection = await _connectionFactory.CreateAsync();
        const string query = 
            """
            SELECT *
            FROM comments
            WHERE recipe_id = @RecipeId
            ORDER BY created_at DESC
            """;
        var comments = await connection.QueryAsync<Comment>(query, new {
            recipeId
        });
        return comments.ToList();
    }
    
    public async Task<Comment?> GetCommentById(Guid commentId)
    {
        using var connection = await _connectionFactory.CreateAsync();
        const string query = "SELECT * FROM comments WHERE Id = @CommentId";
        var comment = await connection.QuerySingleOrDefaultAsync<Comment>(query, new {
            commentId
        });
        return comment;
    }

    public async Task<int> GetCommentCount(Guid recipeId)
    {
        using var connection = await _connectionFactory.CreateAsync();
        
        const string query = "SELECT COUNT(ID) FROM comments WHERE recipe_id = @RecipeId";

        var count = await connection.QuerySingleOrDefaultAsync<int>(query, new {
            recipeId
        });
        return count;
    }

    public async Task<List<(Guid id, int count)>> getCommentCountsByRecipeIds(IEnumerable<Guid> recipeIds)
    {
        using var connection = await _connectionFactory.CreateAsync();
        
        const string query = 
            """
            SELECT recipe_id, COUNT(ID) AS count
            FROM comments
            WHERE recipe_id = ANY(@RecipeIds)
            GROUP BY recipe_id
            """;

        var counts = await connection.QueryAsync<(Guid recipeId, int count)>(query, new {
            recipeIds = recipeIds.ToList()
        });
        return counts.ToList();
    }
    
    #endregion

    #region INSERT
    
    public async Task<Guid> CreateComment(CommentPostBindingModel model)
    {
        using var connection = await _connectionFactory.CreateAsync();
        const string query = 
            """
            INSERT INTO comments (user_id, recipe_id, content)
            VALUES (@UserId, @RecipeId, @Content)
            RETURNING id
            """;
        var commentId = await connection.ExecuteScalarAsync<Guid>(query, new {
            _currentContext.UserId,
            model.RecipeId,
            model.Content,
        });
        return commentId;
    }
    
    #endregion
}
