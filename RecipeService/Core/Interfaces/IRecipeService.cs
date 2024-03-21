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
    /// Retrieves a list of recipes that match the provided tags.
    /// </summary>
    /// <param name="tags">The list of tag ids to match.</param>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects that match the provided tags.</returns>
    Task<List<ListRecipeDto>> GetRecipesByTags(List<Guid> tags);
    
    /// <summary>
    /// Creates a new recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePostBindingModel"/> that contains the information for the new recipe.</param>
    /// <returns>The <see cref="Guid"/> of the newly created recipe.</returns>
    Task<Guid?> CreateRecipe(RecipePostBindingModel model);
    
    /// <summary>
    /// Edits an existing recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePutBindingModel"/> that contains the information for the recipe to be edited.</param>
    /// <returns>The <see cref="Guid"/> of the edited recipe.</returns>
    Task<bool> EditRecipe(RecipePutBindingModel model);
}
