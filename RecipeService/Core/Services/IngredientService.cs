using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Core.Services;

public class IngredientService : IIngredientService {
    private readonly IIngredientRepository _ingredientRepository;
    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    #region Read
    public async Task<List<Ingredient>> GetAllIngredients()
    {
        return await _ingredientRepository.GetAllIngredients();
    }
    
    public async Task<List<Ingredient>> GetIngredientsBySearch(string query)
    {
        return await _ingredientRepository.GetIngredientsBySearch(query);
    }
    #endregion

    #region Create
    
    public async Task<Ingredient?> CreateIngredient(IngredientPostBindingModel model)
    {
        var createdId = await _ingredientRepository.CreateIngredient(model);
        return await _ingredientRepository.GetIngredientById(createdId);
    }
    #endregion
}
