namespace RecipeService.Core.Models.Entities;

public class Recipe {
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Info { get; set; }
    public Guid CreatedById { get; set; }
    public List<Tag> Tags { get; set; } = [];
}