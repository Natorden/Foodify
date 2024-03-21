using RecipeService.Core.Context;
using RecipeService.Core.Interfaces;
using RecipeService.Core.Services;
using RecipeService.Infrastructure.Interfaces;
using RecipeService.Infrastructure.Repositories;
namespace RecipeService.Extensions;

public static class ServicesAndRepositoryExtension
{
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {

        #region Service
        
        services.AddTransient<IRecipeService, Core.Services.RecipeService>();
        services.AddTransient<ITagService, TagService>();
        services.AddTransient<IIngredientService, IngredientService>();
        
        #endregion

        #region Repository
        
        services.AddTransient<IRecipeRepository, RecipeRepository>();
        services.AddTransient<ITagRepository, TagRepository>();
        services.AddTransient<IIngredientRepository, IngredientRepository>();
        
        #endregion
        
        services.AddScoped<CurrentContext>();

        return services;
    }
}
