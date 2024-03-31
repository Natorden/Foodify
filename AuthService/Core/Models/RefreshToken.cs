using Microsoft.EntityFrameworkCore;
namespace AuthService.Core.Models;

[Owned]
public class RefreshToken
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null && !IsExpired;
}
