namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Solicitação para renovar um token JWT expirado
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Token de atualização obtido no login ou refresh anterior
    /// </summary>
    /// <example>a1b2c3d4e5f67890abcdef1234567890...</example>
    public string RefreshToken { get; set; } = string.Empty;
}
