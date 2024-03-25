namespace RecipeService.Core.Models.Entities;

public class RecipeStep {
    public Guid RecipeId { get; set; }
    public int Priority { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}
