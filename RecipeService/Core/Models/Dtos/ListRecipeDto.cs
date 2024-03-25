using RecipeService.Core.Models.Entities;
using Shared.Models;
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
    public required string Image { get; set; }
    public bool IsLiked { get; set; }
    public Guid CreatedById { get; set; }
    public SharedUserProfileDto? CreatedByUser { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public int Likes { get; set; }
    public int? Comments { get; set; }
}