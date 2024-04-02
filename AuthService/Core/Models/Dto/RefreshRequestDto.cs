namespace AuthService.Core.Models.Dto;

public class RefreshRequestDto {
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}
