using System.ComponentModel.DataAnnotations;
namespace RecipeService.Core.Models.BindingModels;

public class RecipePutBindingModel {
    [Required]
    public Guid Id { get; set; }
    [Length(2,32)]
    public string? Title { get; set; }
    [MaxLength(1000)]
    public string? Info { get; set; }
    [Length(1,8)]
    public List<Guid>? Tags { get; set; }
    [Length(1,32)]
    public List<RecipeIngredientPutBindingModel>? Ingredients { get; set; }
    [Length(1,16)]
    public List<RecipeStepPutBindingModel>? Steps { get; set; }
}
