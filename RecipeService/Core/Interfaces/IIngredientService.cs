using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Interfaces;

public interface IIngredientService {
    /// <summary>
    /// Retrieves a list of all ingredients.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Ingredient"/> objects.
    /// </returns>
    Task<List<Ingredient>> GetAllIngredients();
    /// <summary>
    /// Retrieves a list of ingredients based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Ingredient"/> objects.</returns>
    Task<List<Ingredient>> GetIngredientsBySearch(string query);
    /// <summary>
    /// Creates a new ingredient based on the specified binding model.
    /// </summary>
    /// <param name="model">The <see cref="IngredientPostBindingModel"/> containing the new ingredient's information.</param>
    /// <returns>The newly created <see cref="Ingredient"/> object.</returns>
    Task<Ingredient?> CreateIngredient(IngredientPostBindingModel model);
}
