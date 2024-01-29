using AuthService.Core.Models;
using Microsoft.AspNetCore.Identity;
namespace AuthService.Infrastructure.Initialize;

public abstract class DbInitializerHelper
{
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly RoleManager<ApplicationRole> _roleManager;

    protected DbInitializerHelper( UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected async Task Init()
    {
        const string user1Email = "user@app";
        const string user2Email = "user2@app";

        if (_roleManager.Roles.SingleOrDefault(r => r.Name == "User") == null)
        {
            await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = "User"
            });
        }

        // User 1 with a finished period
        var user1 = new ApplicationUser
        {
            Id = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            UserName = user1Email,
            Email = user1Email,
            EmailConfirmed = true,
        };
        await _userManager.CreateAsync(user1, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user1, "User");

        // User 2 with a current period
        var user2 = new ApplicationUser
        {
            Id = Guid.Parse("B1F0B1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            UserName = user2Email,
            Email = user2Email,
            EmailConfirmed = true,
        };

        await _userManager.CreateAsync(user2, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user2, "User");
    }
}
