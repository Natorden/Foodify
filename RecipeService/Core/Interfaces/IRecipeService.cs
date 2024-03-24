using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Interfaces;

public interface IRecipeService {
    
    /// <summary>
    /// Retrieves a recipeDto with the specified ID.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the recipe to retrieve.</param>
    /// <returns>The <see cref="RecipeDto"/> object with the specified ID.</returns>
    Task<RecipeDto?> GetRecipeDtoById(Guid id);
    
    /// <summary>
    /// Retrieves all recipes.
    /// </summary>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects representing the recipes.</returns>
    Task<List<ListRecipeDto>> GetAllRecipes();

    /// <summary>
    /// Retrieves a list of recipes that match the provided tags and title name.
    /// </summary
    /// <param name="query">The search query</param>
    /// <param name="tags">The list of tag ids to match.</param>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects that match the provided tags.</returns>
    Task<List<RecipeSummaryDto>> GetRecipesBySearch(string query, List<Guid> tags);
    
    /// <summary>
    /// Retrieves a list of summaries for recipes that were made by the the specified user.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>A list of <see cref="RecipeSummaryDto"/> objects that match the provided userId.</returns>
    Task<List<RecipeSummaryDto>> GetRecipesCreatedByUser(Guid userId);
    
    /// <summary>
    /// Retrieves a list of summaries for recipes that were liked by the the specified user.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>A list of <see cref="RecipeSummaryDto"/> objects that match the provided userId.</returns>
    Task<List<RecipeSummaryDto>> GetRecipesLikedByUser(Guid userId);
    
    /// <summary>
    /// Creates a new recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePostBindingModel"/> that contains the information for the new recipe.</param>
    /// <returns>The <see cref="Guid"/> of the newly created recipe.</returns>
    Task<Guid?> CreateRecipe(RecipePostBindingModel model);
    
    /// <summary>
    /// Add like of the current user for a recipe with the specified ID.
    /// </summary>
    /// <param name="recipeId">The <see cref="Guid"/> of the recipe to add the like to.</param>
    /// <returns>A <see cref="bool"/> value indicating whether the like was successfully added.</returns>
    Task<bool> LikeRecipe(Guid recipeId);
    
    /// <summary>
    /// Edits an existing recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePutBindingModel"/> that contains the information for the recipe to be edited.</param>
    /// <returns>The <see cref="Guid"/> of the edited recipe.</returns>
    Task<bool> EditRecipe(RecipePutBindingModel model);
    
    /// <summary>
    /// Removes the like of the current user for a recipe with the specified ID.
    /// </summary>
    /// <param name="recipeId">The <see cref="Guid"/> of the recipe to remove the like from.</param>
    /// <returns>A <see cref="bool"/> value indicating whether the like was successfully removed.</returns>
    Task<bool> UnlikeRecipe(Guid recipeId);
}
