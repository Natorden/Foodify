using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Interfaces;

public interface IRecipeService {
    
    /// <summary>
    /// Retrieves a recipe with the specified ID.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the recipe to retrieve.</param>
    /// <returns>The <see cref="Recipe"/> object with the specified ID.</returns>
    Task<Recipe?> GetRecipeById(Guid id);
    
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
}
