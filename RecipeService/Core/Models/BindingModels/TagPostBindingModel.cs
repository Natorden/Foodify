namespace RecipeService.Core.Models.BindingModels;

/// <summary>
/// Represents the binding model for creating a new tag.
/// </summary>
public class TagPostBindingModel {
    public required string Name { get; set; }
    public required string Description { get; set; }
}
