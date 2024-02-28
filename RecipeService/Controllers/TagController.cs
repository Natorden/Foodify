using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Core.Models.Exceptions;
namespace RecipeService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase {
    private readonly ITagService _tagService;
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    #region GET
    /// <summary>
    /// Retrieves a list of all tags.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Tag"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _tagService.GetAllTags();
        return Ok(tags);
    }
    /// <summary>
    /// Retrieves a list of tags based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Tag"/> objects.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> GetTagsBySearch([Required] [FromQuery] string query)
    {
        var tags = await _tagService.GetTagsBySearch(query);
        return Ok(tags);
    }
    
    #endregion

    #region POST
    /// <summary>
    /// Creates a new tag based on the specified binding model.
    /// </summary>
    /// <param name="model">The <see cref="TagPostBindingModel"/> containing the new tag's information.</param>
    /// <returns>The newly created <see cref="Tag"/> object.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTag(TagPostBindingModel model)
    {
        var tag = await _tagService.CreateTag(model);
        if (tag is null) {
            throw new BadRequestException("Failed to create new tag");
        }
        return Ok(tag);
    }
    
    #endregion
}