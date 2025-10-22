namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Solicita��o para renovar um token JWT expirado
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Token de atualiza��o obtido no login ou refresh anterior
    /// </summary>
    /// <example>a1b2c3d4e5f67890abcdef1234567890...</example>
    public string RefreshToken { get; set; } = string.Empty;
}
