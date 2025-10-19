namespace LocadoraLivros.Api.Models.DTOs.Auth;

public class AuthResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiresAt { get; set; }
}
