using Dapper;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public TagRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region SELECT
    
    public async Task<List<Tag>> GetAllTags()
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string query =
            """
            SELECT * FROM tags;
            """;
        var tags = await conn.QueryAsync<Tag>(query);
        return tags.ToList();
    }
    
    public async Task<List<Tag>> GetTagsBySearch(string query)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
            SELECT * FROM tags
            WHERE Name ILIKE @Query;
            """;
        var tags = await conn.QueryAsync<Tag>(sql, new {
            Query = $"%{query}%"
        });
        return tags.ToList();
    }

    public async Task<Tag?> GetTagById(Guid guid)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
            SELECT * FROM tags
            WHERE Id = @Id;
            """;
        var tag = await conn.QuerySingleOrDefaultAsync<Tag>(sql, new {
            Id = guid
        });
        return tag;
    }
    #endregion

    #region INSERT
    
    public async Task<Guid> CreateTag(TagPostBindingModel model)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
            INSERT INTO tags (Id, Name)
            VALUES (@Id, @Name)
            RETURNING id;
            """;
        var tagId = await conn.ExecuteScalarAsync<Guid>(sql, new {
            Id = Guid.NewGuid(),
            model.Name,
        });
        return tagId;
    }
    
    #endregion
}
