using System.ComponentModel.DataAnnotations;
namespace RecipeService.Core.Models.BindingModels;

public class RecipeStepPutBindingModel {
    [Required]
    [Range(1,16)]
    public int Priority { get; set; }
    [Required]
    [Length(1,32)]
    public required string Title { get; set; }
    [Required]
    [Length(1,256)]
    public required string Description { get; set; }
}
