namespace RecipeService.Core.Models.BindingModels;

/// <summary>
/// Represents the binding model for creating a new ingredient.
/// </summary>
public class IngredientPostBindingModel {
    public required string Name { get; set; }
}
