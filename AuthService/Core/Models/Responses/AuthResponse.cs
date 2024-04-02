namespace AuthService.Core.Models.Responses;

public class AuthResponse
{
    public string Email { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshExpiresAt { get; set; }
    public List<string> Roles { get; set; } = [];
}
