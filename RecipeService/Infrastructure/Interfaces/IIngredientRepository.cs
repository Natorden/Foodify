using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Infrastructure.Interfaces;

public interface IIngredientRepository {
    /// <summary>
    /// Retrieves all ingredients from the ingredient repository.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Ingredient"/> objects representing all the ingredients in the repository.
    /// </returns>
    Task<List<Ingredient>> GetAllIngredients();
    
    /// <summary>
    /// Retrieves a list of ingredients based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Ingredient"/> objects.</returns>
    Task<List<Ingredient>> GetIngredientsBySearch(string query);
    
    /// <summary>
    /// Retrieves a Ingredient object from the ingredient repository based on the provided ID.
    /// </summary>
    /// <param name="guid">The ID of the ingredient to retrieve.</param>
    /// <returns>The <see cref="Ingredient"/> object matching the provided ID.</returns>
    Task<Ingredient?> GetIngredientById(Guid guid);
    
    /// <summary>
    /// Creates a new ingredient in the ingredient repository.
    /// </summary>
    /// <param name="model">The <see cref="IngredientPostBindingModel"/> object containing the new ingredient's data.</param>
    /// <returns>The <see cref="Guid"/> of the newly created <see cref="Ingredient"/>.</returns>
    Task<Guid> CreateIngredient(IngredientPostBindingModel model);
}
