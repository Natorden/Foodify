using RecipeService.Core.Models.BindingModels;
using RecipeService.Core.Models.Entities;
namespace RecipeService.Infrastructure.Interfaces;

public interface ITagRepository {
    /// <summary>
    /// Retrieves all tags from the tag repository.
    /// </summary>
    /// <returns>
    /// A list of Tag objects representing all the tags in the repository.
    /// </returns>
    Task<List<Tag>> GetAllTags();
    
    /// <summary>
    /// Retrieves a list of tags based on a search query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of <see cref="Tag"/> objects.</returns>
    Task<List<Tag>> GetTagsBySearch(string query);
    /// <summary>
    /// Retrieves a Tag object from the tag repository based on the provided ID.
    /// </summary>
    /// <param name="guid">The ID of the tag to retrieve.</param>
    /// <returns>The Tag object matching the provided ID.</returns>
    Task<Tag?> GetTagById(Guid guid);
    /// <summary>
    /// Creates a new tag in the tag repository.
    /// </summary>
    /// <param name="model">The <see cref="TagPostBindingModel"/> object containing the new tag's data.</param>
    /// <returns>The <see cref="Guid"/> of the newly created <see cref="Tag"/>.</returns>
    Task<Guid> CreateTag(TagPostBindingModel model);
}
