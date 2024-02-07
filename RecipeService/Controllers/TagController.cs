using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
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
    
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _tagService.GetAllTags();
        return Ok(tags);
    }
    
    [HttpGet("search/{query}")]
    public async Task<IActionResult> GetTagsBySearch(string query)
    {
        var tags = await _tagService.GetTagsBySearch(query);
        return Ok(tags);
    }
    
    #endregion

    #region POST

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