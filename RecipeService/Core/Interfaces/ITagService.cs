using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Interfaces;

public interface ITagService {
    /// <summary>
    /// Retrieves a list of all tags.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Tag"/> objects.
    /// </returns>
    Task<List<Tag>> GetAllTags();
    /// <summary>
    /// Retrieves a list of tags based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Tag"/> objects.</returns>
    Task<List<Tag>> GetTagsBySearch(string query);
    /// <summary>
    /// Creates a new tag based on the specified binding model.
    /// </summary>
    /// <param name="model">The <see cref="TagPostBindingModel"/> containing the new tag's information.</param>
    /// <returns>The newly created <see cref="Tag"/> object.</returns>
    Task<Tag?> CreateTag(TagPostBindingModel model);
}
