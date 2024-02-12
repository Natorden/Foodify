using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Models.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for a recipe item in a list. <para/>
/// </summary>
/// <remarks>
/// Simplified version of a recipe, intended to be shown in a large list, as a result of a search
/// </remarks>
public class ListRecipeDto {
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public Guid CreatedById { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public int Likes { get; set; }
    public int Comments { get; set; }
    // public required string ImageUrl { get; set; }
}
