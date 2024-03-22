using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
using RecipeService.Core.Models.Exceptions;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Core.Services;

public class RecipeService : IRecipeService {
    private readonly IRecipeRepository _recipeRepository;
    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    #region Read
    
    public async Task<RecipeDto?> GetRecipeDtoById(Guid id)
    {
        return await _recipeRepository.GetRecipeDtoById(id);
    }
    public async Task<List<ListRecipeDto>> GetAllRecipes()
    {
        var recipes = await _recipeRepository.GetAllRecipes();
        // TODO: This is how I would map it Mr. Rolf
        /*recipes.ForEach(recipe =>
        {
            recipe.CreatedBy = getUserById(recipe.CreatedById);
        });*/
        return recipes;
    }
    public async Task<List<ListRecipeDto>> GetRecipesByTags(List<Guid> tags)
    {
        return await _recipeRepository.GetRecipesByTags(tags);
    }
    
    public async Task<bool> LikeRecipe(Guid recipeId)
    {
        var recipe = await _recipeRepository.GetRecipeDtoById(recipeId);
        if (recipe is null) {
            throw new NotFoundException("Recipe not found");
        }
        if (recipe.IsLiked) {
            throw new BadRequestException("Recipe is already liked");
        }
        return await _recipeRepository.LikeRecipe(recipeId);
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

    #region Delete
    
    public async Task<bool> UnlikeRecipe(Guid recipeId)
    {
        var recipe = await _recipeRepository.GetRecipeDtoById(recipeId);
        if (recipe is null) {
            throw new NotFoundException("Recipe not found");
        }
        if (!recipe.IsLiked) {
            throw new BadRequestException("Recipe is not liked");
        }
        return await _recipeRepository.UnlikeRecipe(recipeId);
    }
    
    #endregion
}
