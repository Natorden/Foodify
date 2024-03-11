using RecipeService.Core.Models.Enums;
namespace RecipeService.Core.Models.BindingModels;

public class RecipeIngredientPutBindingModel {
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    public MeasurementUnits Unit { get; set; }
}
