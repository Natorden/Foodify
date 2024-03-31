using AuthService.Core.Models;
namespace AuthService.Core.Services.Interfaces;

public interface IJwtService 
{
    string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles,
        IDictionary<string, dynamic>? customClaims);
    string GenerateRefreshToken(ApplicationUser user);
}
