using RecipeService.Core.Models.Entities;
namespace RecipeService.Core.Models.Dtos;

/// <summary>
/// Represents a data transfer object of a recipe summery.
/// </summary>
/// <remarks>
/// This Dto is used in lists that contain only basic recipe information 
/// </remarks>
public class RecipeSummaryDto {
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Image { get; set; }
    public List<Tag> Tags { get; set; } = [];
}
