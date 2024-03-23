using System.Data;
using Dapper;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Context;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly CurrentContext _currentContext;
    public RecipeRepository(IDbConnectionFactory connectionFactory, CurrentContext currentContext)
    {
        _connectionFactory = connectionFactory;
        _currentContext = currentContext;
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
    
    public async Task<RecipeDto?> GetRecipeDtoById(Guid id)
    {
        using var conn = await _connectionFactory.CreateAsync();
        RecipeDto? recipeModel = null;
        HashSet<string> recipeImages = [];
        HashSet<Guid> recipeTagIds = [];
        HashSet<Guid> recipeIngredientIds = [];
        HashSet<int> recipeStepPriorities = [];
        
        const string query =
            """
            SELECT
                r.id, r.title, r.info, r.created_by_id, 
                CASE WHEN rl.user_id is NULL THEN 0 ELSE 1 END AS is_Liked,
                rimg.url,
                t.id, t.name,
                i.id, i.name, ri.unit, ri.amount,
                rs.title, rs.description, rs.priority
            FROM recipes r
            JOIN recipe_images rimg ON r.id = rimg.recipe_id
            JOIN recipe_tags rt ON r.id = rt.recipe_id
            JOIN tags t ON t.id = rt.tag_id
            JOIN recipe_steps rs ON r.id = rs.recipe_id
            JOIN recipe_ingredients ri ON r.id = ri.recipe_id
            JOIN ingredients i ON i.id = ri.ingredient_id
            LEFT JOIN recipe_likes rl ON rl.recipe_id = r.id AND rl.user_id = @userId
            WHERE r.id = @id
            ORDER BY rimg.priority, rt.priority, ri.priority, rs.priority
            """;
        await conn.QueryAsync<RecipeDto, string, Tag, RecipeIngredientDto, RecipeStepDto, int, RecipeDto>(
            query,
            (recipe, image, tag, ingredient, step, stepPriority) =>
            {
                recipeModel ??= recipe;
                if (recipeImages.Add(image)) recipeModel.Images.Add(image);
                if (recipeTagIds.Add(tag.Id)) recipeModel.Tags.Add(tag);
                if (recipeIngredientIds.Add(ingredient.Id)) recipeModel.Ingredients.Add(ingredient);
                if (recipeStepPriorities.Add(stepPriority)) recipeModel.Steps.Add(step);
                return recipe;
            },
            new {
                id,
                _currentContext.UserId
            },
            splitOn: "id,url,id,id,title,priority");
        return recipeModel;
    }
    
    public async Task<List<ListRecipeDto>> GetAllRecipes()
    {
        using var conn = await _connectionFactory.CreateAsync();
        var dictionary = new Dictionary<Guid, ListRecipeDto>();
        const string query =
            """
            WITH like_counts AS (
                SELECT recipe_id, Count(user_id) AS likes 
                FROM recipe_likes 
                GROUP BY recipe_id 
            )            

            SELECT 
                r.Id, title, created_by_id, rimg.url AS image, likes,
                CASE WHEN rl.user_id is NULL THEN 0 ELSE 1 END AS is_Liked,
                t.id, name
            FROM recipes r
            JOIN recipe_images rimg ON r.id = rimg.recipe_id
            JOIN recipe_tags rt ON r.id = rt.recipe_id
            JOIN tags t ON t.id = rt.tag_id
            LEFT JOIN recipe_likes rl ON rl.recipe_id = r.id AND rl.user_id = @userId
            LEFT JOIN like_counts ON like_counts.recipe_id = r.id 
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
                _currentContext.UserId
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
                VALUES (@Title, @Info, @UserId)
                RETURNING id;
            """;
        var createdId = await conn.ExecuteScalarAsync<Guid>(
            createRecipeSql,
            new {
                model.Title,
                model.Info,
                _currentContext.UserId
            },
            transaction
        );
        
        if (!await CreateRecipeImages(model.Images, conn, createdId, transaction)) return null;
        
        if (!await CreateRecipeTags(model.Tags, conn, createdId, transaction)) return null;

        if (!await CreateRecipeSteps(model.Steps, conn, createdId, transaction)) return null;

        if (!await CreateRecipeIngredients(model.Ingredients, conn, createdId, transaction)) return null;

        transaction.Commit();
        return createdId;
    }
    
    public async Task<bool> LikeRecipe(Guid recipeId)
    {
        using var conn = await _connectionFactory.CreateAsync();
        
        const string createRecipeSql = 
            """
            INSERT INTO recipe_likes (recipe_id, user_id)
            VALUES (@RecipeId, @UserId)
            """;
        var rowsAffected = await conn.ExecuteAsync(
            createRecipeSql,
            new {
                recipeId,
                _currentContext.UserId
            }
        );

        return rowsAffected == 1;
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

    #region DELETE
    
    public async Task<bool> UnlikeRecipe(Guid recipeId)
    {
        using var conn = await _connectionFactory.CreateAsync();
        
        const string createRecipeSql = 
            """
            DELETE FROM recipe_likes
            WHERE recipe_id = @RecipeId
              AND User_id = @UserId
            """;
        var rowsAffected = await conn.ExecuteAsync(
            createRecipeSql,
            new {
                recipeId,
                _currentContext.UserId
            }
        );

        return rowsAffected == 1;
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
