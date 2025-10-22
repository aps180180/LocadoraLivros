namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Solicitação para revogar/invalidar um refresh token
/// </summary>
public class RevokeTokenDto
{
    /// <summary>
    /// Token de atualização a ser revogado
    /// </summary>
    /// <example>a1b2c3d4e5f67890abcdef1234567890...</example>
    public string RefreshToken { get; set; } = string.Empty;
}
