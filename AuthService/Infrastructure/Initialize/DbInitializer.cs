using AuthService.Core.Models;
using Microsoft.AspNetCore.Identity;
namespace AuthService.Infrastructure.Initialize;

public class DbInitializer : DbInitializerHelper
{
    public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : base(userManager, roleManager)
    {
    }
    public new Task Init()
    {
        return base.Init();
    }
}
