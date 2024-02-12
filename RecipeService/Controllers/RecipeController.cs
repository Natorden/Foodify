using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.Dtos;
using RecipeService.Core.Models.Entities;
using RecipeService.Core.Models.Exceptions;

namespace RecipeService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipeController : ControllerBase {
    private readonly IRecipeService _recipeService;
    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    #region GET
    
    /// <summary>
    /// Retrieves a recipe with the specified ID.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the recipe to retrieve.</param>
    /// <returns>The <see cref="Recipe"/> object with the specified ID.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecipeById(Guid id)
    {
        var recipe = await _recipeService.GetRecipeById(id);
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
    public async Task<List<ListRecipeDto>> GetAllRecipes()
    {
        return await _recipeService.GetAllRecipes();
    }
    
    /// <summary>
    /// Retrieves a list of recipes that match the provided tags.
    /// </summary>
    /// <param name="tagIds">The list of tag ids to match.</param>
    /// <returns>A list of <see cref="ListRecipeDto"/> objects that match the provided tags.</returns>
    [HttpGet("tags")]
    public async Task<List<ListRecipeDto>> GetRecipesByTags([FromBody] List<Guid> tagIds) 
    {
        return await _recipeService.GetRecipesByTags(tagIds);
    }
    
    #endregion
}