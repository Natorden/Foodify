using RecipeService.Core.Interfaces;
using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
using RecipeService.Infrastructure.Interfaces;
namespace RecipeService.Core.Services;

public class TagService : ITagService {
    private readonly ITagRepository _tagRepository;
    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    #region Read
    public async Task<List<Tag>> GetAllTags()
    {
        return await _tagRepository.GetAllTags();
    }
    
    public async Task<List<Tag>> GetTagsBySearch(string query)
    {
        return await _tagRepository.GetTagsBySearch(query);
    }
    #endregion

    #region Create
    
    public async Task<Tag?> CreateTag(TagPostBindingModel model)
    {
        var createdId = await _tagRepository.CreateTag(model);
        return await _tagRepository.GetTagById(createdId);
    }
    #endregion
}
