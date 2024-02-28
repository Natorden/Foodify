using Dapper;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public IngredientRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region SELECT
    
    public async Task<List<Ingredient>> GetAllIngredients()
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string query =
            """
                SELECT * FROM ingredients;
            """;
        var ingredients = await conn.QueryAsync<Ingredient>(query);
        return ingredients.ToList();
    }
    
    public async Task<List<Ingredient>> GetIngredientsBySearch(string query)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
                SELECT * FROM ingredients
                WHERE Name ILIKE @Query;
            """;
        var ingredients = await conn.QueryAsync<Ingredient>(sql, new {
            Query = $"%{query}%"
        });
        return ingredients.ToList();
    }

    public async Task<Ingredient?> GetIngredientById(Guid guid)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
                SELECT * FROM ingredients
                WHERE Id = @Id;
            """;
        var tag = await conn.QuerySingleOrDefaultAsync<Ingredient>(sql, new {
            Id = guid
        });
        return tag;
    }
    #endregion

    #region INSERT
    
    public async Task<Guid> CreateIngredient(IngredientPostBindingModel model)
    {
        using var conn = await _connectionFactory.CreateAsync();
        const string sql = 
            """
                INSERT INTO ingredients (Id, Name)
                VALUES (@Id, @Name)
                RETURNING id;
            """;
        var tagId = await conn.ExecuteScalarAsync<Guid>(sql, new {
            Id = Guid.NewGuid(),
            model.Name
        });
        return tagId;
    }
    
    #endregion
}
