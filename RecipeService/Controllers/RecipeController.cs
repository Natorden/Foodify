using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Exceptions;

namespace RecipeService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecipeController : ControllerBase {
    private readonly IRecipeService _recipeService;
    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    #region GET
    
    /// <summary>
    /// Retrieves a recipeDto with the specified ID.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the recipe to retrieve.</param>
    /// <returns>The <see cref="RecipeDto"/> object with the specified ID.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecipeById(Guid id)
    {
        var recipe = await _recipeService.GetRecipeDtoById(id);
        if (recipe == null) {
            throw new NotFoundException("Recipe not found");
        }
        return Ok(recipe);
    }

    /// <summary>
    /// Retrieves all recipes.
    /// </summary>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects representing the recipes.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllRecipes()
    {
        var recipes = await _recipeService.GetAllRecipes();
        return Ok(recipes);
    }
    
    /// <summary>
    /// Retrieves a list of recipes that match the provided tags.
    /// </summary>
    /// <param name="tagIds">The list of tag ids to match.</param>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects that match the provided tags.</returns>
    [HttpGet("tags")]
    public async Task<IActionResult> GetRecipesByTags([FromBody] List<Guid> tagIds) 
    {
        var recipes = await _recipeService.GetRecipesByTags(tagIds);
        return Ok(recipes);
    }
    
    #endregion
    
    #region POST

    /// <summary>
    /// Creates a new recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePostBindingModel"/> that contains the information for the new recipe.</param>
    /// <returns>The <see cref="Guid"/> of the newly created recipe.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRecipe(RecipePostBindingModel model)
    {
        var recipeId = await _recipeService.CreateRecipe(model);
        if (recipeId is null) {
            throw new BadRequestException("Failed to create recipe");
        }
        return Ok(recipeId);
    }
    
    #endregion

    #region PUT

    /// <summary>
    /// Edits an existing recipe with the provided binding model.
    /// </summary>
    /// <param name="model">The <see cref="RecipePutBindingModel"/> that contains the information for the recipe to be edited.</param>
    [HttpPut]
    public async Task<IActionResult> EditRecipe(RecipePutBindingModel model)
    {
        var success = await _recipeService.EditRecipe(model);
        if (!success) {
            throw new BadRequestException("Failed to edit recipe");
        }
        return NoContent();
    }
    
    #endregion
}