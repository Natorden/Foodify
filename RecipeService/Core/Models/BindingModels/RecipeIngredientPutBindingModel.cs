using RecipeService.Core.Models.Enums;
namespace RecipeService.Core.Models.BindingModels;

public class RecipeIngredientPutBindingModel {
    public Guid IngredientId { get; set; }
    public int Amount { get; set; }
    public MeasurementUnits Units { get; set; }
}
