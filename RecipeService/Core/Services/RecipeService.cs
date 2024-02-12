using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Core.Services;

public class RecipeService : IRecipeService {
    private readonly IRecipeRepository _recipeRepository;
    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    #region Read
    
    public async Task<Recipe?> GetRecipeById(Guid id)
    {
        return await _recipeRepository.GetRecipeById(id);
    }
    public async Task<List<ListRecipeDto>> GetAllRecipes()
    {
        return await _recipeRepository.GetAllRecipes();
    }
    public async Task<List<ListRecipeDto>> GetRecipesByTags(List<Guid> tags)
    {
        return await _recipeRepository.GetRecipesByTags(tags);
    }
    
    #endregion

    #region Create
    
    public async Task<Guid?> CreateRecipe(RecipePostBindingModel model)
    {
        return await _recipeRepository.CreateRecipe(model);
    }
    
    #endregion

    #region Update
    
    public async Task<bool> EditRecipe(RecipePutBindingModel model)
    {
        return await _recipeRepository.EditRecipe(model);
    }
    
    #endregion
}
