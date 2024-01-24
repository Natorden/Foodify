using Microsoft.AspNetCore.Identity;
namespace AuthService.Core.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset? ResetPasswordTokenExpirationDate { get; set; } 
    public DateTimeOffset? VerifyEmailTokenExpirationDate { get; set; }
}
