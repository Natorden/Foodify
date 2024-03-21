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
        const string query = "SELECT * FROM comments WHERE recipe_id = @RecipeId";
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
