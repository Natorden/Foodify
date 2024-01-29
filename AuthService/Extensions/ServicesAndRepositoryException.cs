using AuthService.Core.Context;
using AuthService.Core.Services;
using AuthService.Core.Services.Interfaces;
using AuthService.Infrastructure.Initialize;
namespace AuthService.Extensions;

public static class ServicesAndRepositoryExtension
{
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {

        #region Service
        
        services.AddTransient<IJwtService, JwtService>();

        #endregion

        services.AddScoped<CurrentContext>();
        
        services.AddScoped<DbInitializer>();

        return services;
    }
}
