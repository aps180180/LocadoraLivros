namespace LocadoraLivros.Api.Models.DTOs.Auth;

public class RevokeTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
