using System.Data;
using Dapper;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Models.BindingModels;
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
                WHERE r.id = @id
            """;
        await conn.QueryAsync<Recipe, Tag, Recipe>(
            query,
            (recipe, tag) =>
            {
                recipeModel ??= recipe;
                recipeModel.Tags.Add(tag);
                return recipe;
            },
            new {id});
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

    #region INSERT
    
    public async Task<Guid?> CreateRecipe(RecipePostBindingModel model)
    {
        using var conn = await _connectionFactory.CreateAsync();
        using var transaction = conn.BeginTransaction();
        // Create recipe model
        const string createRecipeSql = 
            """
                INSERT INTO recipes (title, info, created_by_id)
                VALUES (@Title, @Info, gen_random_uuid()) --TODO: use logged in user ID
                RETURNING id;
            """;
        var createdId = await conn.ExecuteScalarAsync<Guid>(createRecipeSql, model,transaction);
        
        if (!await CreateRecipeImages(model.Images, conn, createdId, transaction)) return null;
        
        if (!await CreateRecipeTags(model.Tags, conn, createdId, transaction)) return null;

        if (!await CreateRecipeSteps(model.Steps, conn, createdId, transaction)) return null;

        if (!await CreateRecipeIngredients(model.Ingredients, conn, createdId, transaction)) return null;

        transaction.Commit();
        return createdId;
    }
    
    #endregion

    #region UPDATE
    
    public async Task<bool> EditRecipe(RecipePutBindingModel model)
    {
        using var conn = await _connectionFactory.CreateAsync();
        using var transaction = conn.BeginTransaction();
        // Update recipe model
        const string updateRecipeSql = 
            """
                UPDATE recipes SET
                title = COALESCE(@title,title),
                info = COALESCE(@info,info)
                WHERE id = @id;
            """;
        var rowsUpdated = await conn.ExecuteAsync(updateRecipeSql, model, transaction);
        if (rowsUpdated == 0) {
            transaction.Rollback();
            return false;
        }
        if (model.Images is not null) {
            await conn.ExecuteAsync(
                "DELETE FROM recipe_images WHERE recipe_id = @recipeId;",
                new {recipeId = model.Id},
                transaction);
            
            if (!await CreateRecipeImages(model.Images, conn, model.Id, transaction)) return false;
        }
        if (model.Tags is not null) {
            await conn.ExecuteAsync(
                "DELETE FROM recipe_tags WHERE recipe_id = @recipeId;",
                new {recipeId = model.Id},
                transaction);
            
            if (!await CreateRecipeTags(model.Tags, conn, model.Id, transaction)) return false;
        }
        if (model.Steps is not null) {
            await conn.ExecuteAsync(
                "DELETE FROM recipe_steps WHERE recipe_id = @recipeId;",
                new {recipeId = model.Id},
                transaction);
            
            if (!await CreateRecipeSteps(model.Steps, conn, model.Id, transaction)) return false;
        }
        if (model.Ingredients is not null) {
            await conn.ExecuteAsync(
                "DELETE FROM recipe_ingredients WHERE recipe_id = @recipeId;",
                new {recipeId = model.Id},
                transaction);
            
            if (!await CreateRecipeIngredients(model.Ingredients, conn, model.Id, transaction)) return false;
        }
        transaction.Commit();
        return true;
    }
    
    #endregion

    #region Private methods
    
    private async static Task<bool> CreateRecipeTags(IReadOnlyCollection<Guid> tags, IDbConnection conn, Guid createdId, IDbTransaction transaction)
    {
        const string createTagsSql = 
            """
                INSERT INTO recipe_tags (recipe_id, tag_id, priority)
                VALUES (@recipeId, @tagId, @priority);
            """;
        
        var rowsAffectedTags = await conn.ExecuteAsync(
            createTagsSql, 
            tags.Select((tag, index) => 
                new {
                    tagId = tag,
                    recipeId = createdId,
                    priority = index
                }),
            transaction);
        
        if (rowsAffectedTags != tags.Count) {
            transaction.Rollback();
            return false;
        }
        return true;
    }
    private async static Task<bool> CreateRecipeSteps(List<RecipeStepPutBindingModel> steps, IDbConnection conn, Guid createdId, IDbTransaction transaction)
    {
        const string createStepsSql = 
            """
                INSERT INTO recipe_steps (recipe_id, priority, title, description)
                VALUES (@recipeId, @priority, @title, @description);
            """;
        
        var rowsAffectedSteps = await conn.ExecuteAsync(
            createStepsSql, 
            steps.Select((step, index) => 
                new {
                    recipeId = createdId,
                    priority = index,
                    step.Title,
                    step.Description
                }),
            transaction);
        
        if (rowsAffectedSteps != steps.Count) {
            transaction.Rollback();
            return false;
        }
        return true;
    }
    private async static Task<bool> CreateRecipeIngredients(List<RecipeIngredientPutBindingModel> ingredients, IDbConnection conn, Guid createdId, IDbTransaction transaction)
    {
        const string createIngredientsSql = 
            """
                INSERT INTO recipe_ingredients (recipe_id, ingredient_id, amount, unit, priority)
                VALUES (@recipeId, @ingredientId, @amount, @unit, @priority);
            """;
        var rowsAffectedIngredients = await conn.ExecuteAsync(createIngredientsSql, 
            ingredients.Select((ingredient, index) => new {
                recipeId = createdId,
                ingredient.IngredientId,
                ingredient.Amount,
                ingredient.Unit,
                priority = index
            }), transaction);
        if (rowsAffectedIngredients != ingredients.Count) {
            transaction.Rollback();
            return false;
        }
        return true;
    }
    private async static Task<bool> CreateRecipeImages(List<string> images, IDbConnection conn, Guid createdId, IDbTransaction transaction)
    {
        const string createIngredientsSql = 
            """
                INSERT INTO recipe_images (recipe_id, url, priority)
                VALUES (@recipeId, @url, @priority);
            """;
        var rowsAffectedIngredients = await conn.ExecuteAsync(createIngredientsSql, 
            images.Select((imageUrl, index) => new {
                recipeId = createdId,
                url = imageUrl,
                priority = index
            }), transaction);
        if (rowsAffectedIngredients != images.Count) {
            transaction.Rollback();
            return false;
        }
        return true;
    }
    
    #endregion
}
