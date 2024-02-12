using Dapper;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public RecipeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region SELECT
    
    public async Task<Recipe?> GetRecipeById(Guid id)
    {
        using var conn = await _connectionFactory.CreateAsync();
        Recipe? recipeModel = null;
        const string query =
            """
                SELECT r.*, t.*
                FROM recipes r
                LEFT JOIN recipe_tags rt ON r.id = rt.recipe_id
                LEFT JOIN tags t ON t.id = rt.tag_id
                WHERE r.id = @recipeId
            """;
        await conn.QueryAsync<Recipe, Tag, Recipe>(
            query,
            (recipe, tag) =>
            {
                recipeModel ??= recipe;
                recipeModel.Tags.Add(tag);
                return recipe;
            });
        return recipeModel;
    }
    
    public async Task<List<ListRecipeDto>> GetAllRecipes()
    {
        using var conn = await _connectionFactory.CreateAsync();
        var dictionary = new Dictionary<Guid, ListRecipeDto>();
        const string query =
            """
                SELECT r.Id, title, created_by_id, t.*
                FROM recipes r
                LEFT JOIN recipe_tags rt ON r.id = rt.recipe_id
                LEFT JOIN tags t ON t.id = rt.tag_id;
            """;
        await conn.QueryAsync<ListRecipeDto,Tag,ListRecipeDto>(
            query,
            (recipe, tag) =>
            {
                if (dictionary.TryGetValue(recipe.Id, out var recipeMaster)) {
                    recipe = recipeMaster;
                } else {
                    dictionary.Add(recipe.Id,recipe);
                }
                recipe.Tags.Add(tag);
                return recipe;
            });
        return dictionary.Values.ToList();
    }
    
    public async Task<List<ListRecipeDto>> GetRecipesByTags(List<Guid> tags)
    {
        using var conn = await _connectionFactory.CreateAsync();
        var dictionary = new Dictionary<Guid, ListRecipeDto>();
        const string query =
            """
                WITH matching_tags AS (
                    SElECT recipe_id, COUNT(tag_id) AS matches FROM recipe_tags rt
                    WHERE tag_id IN @tagIds
                    GROUP BY recipe_id
                )
                
                SELECT r.Id,title,created_by_id, t.*
                FROM recipes r
                LEFT JOIN recipe_tags rt ON r.id = rt.recipe_id
                LEFT JOIN tags t ON t.id = rt.tag_id
                JOIN matching_tags ON r.id = matching_tags.recipe_id
                ORDER BY matching_tags.matches DESC;
            """;
        await conn.QueryAsync<ListRecipeDto,Tag,ListRecipeDto>(
            query,
            (recipe, tag) =>
            {
                if (dictionary.TryGetValue(recipe.Id, out var recipeMaster)) {
                    recipe = recipeMaster;
                } else {
                    dictionary.Add(recipe.Id,recipe);
                }
                recipe.Tags.Add(tag);
                return recipe;
            },
            new {
                tagIds = tags
            });
        return dictionary.Values.ToList();
    }
    
    #endregion
}
