using AuthService.Core.Models;
using AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Extensions;

public static class IdentityBuilder
{
    public static IServiceCollection SetupIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Sign in settings.
                options.SignIn.RequireConfirmedEmail = false;
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                options.Stores.ProtectPersonalData = false; // nah
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
