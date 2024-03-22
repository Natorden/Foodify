using RecipeService.Core.Models.Entities;
using RecipeService.Core.Models.Enums;
namespace RecipeService.Core.Models.Dtos;

public class RecipeDto {
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Info { get; set; }
    public bool IsLiked { get; set; }
    public Guid CreatedById { get; set; }
    public List<string> Images { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<RecipeIngredientDto> Ingredients { get; set; } = [];
    public List<RecipeStepDto> Steps { get; set; } = [];
}

public class RecipeIngredientDto {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public MeasurementUnits Unit { get; set; }
    public decimal Amount { get; set; }
}

public class RecipeStepDto {
    public required string Title { get; set; }
    public required string Description { get; set; }
}
