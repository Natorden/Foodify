using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Core.Models.Exceptions;
namespace RecipeService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngredientController : ControllerBase {
    private readonly IIngredientService _ingredientService;
    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    #region GET
    /// <summary>
    /// Retrieves a list of all ingredients.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Ingredient"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetIngredients()
    {
        var ingredients = await _ingredientService.GetAllIngredients();
        return Ok(ingredients);
    }
    /// <summary>
    /// Retrieves a list of ingredients based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Ingredient"/> objects.</returns>
    [HttpGet("search/{query}")]
    public async Task<IActionResult> GetIngredientsBySearch(string query)
    {
        var ingredients = await _ingredientService.GetIngredientsBySearch(query);
        return Ok(ingredients);
    }
    
    #endregion

    #region POST

    [HttpPost]
    public async Task<IActionResult> CreateIngredient(IngredientPostBindingModel model)
    {
        var ingredient = await _ingredientService.CreateIngredient(model);
        if (ingredient is null) {
            throw new BadRequestException("Failed to create new ingredient");
        }
        return Ok(ingredient);
    }
    
    #endregion
}