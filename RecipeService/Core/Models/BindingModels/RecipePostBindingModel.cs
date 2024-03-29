using System.ComponentModel.DataAnnotations;
namespace RecipeService.Core.Models.BindingModels;

public class RecipePostBindingModel {
    [Required]
    [Length(2,32)]
    public required string Title { get; set; }
    [Required]
    [MaxLength(2048)]
    public required string Info { get; set; }
    [Required]
    [Length(1,6)]
    public required List<string> Images { get; set; }
    [Required]
    [Length(1,8)]
    public required List<Guid> Tags { get; set; }
    [Required]
    [Length(1,32)]
    public required List<RecipeIngredientPutBindingModel> Ingredients { get; set; }
    [Required]
    [Length(1,16)]
    public required List<RecipeStepPutBindingModel> Steps { get; set; }
}
