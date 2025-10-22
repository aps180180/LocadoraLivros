namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Resposta de autentica��o contendo dados do usu�rio e tokens JWT
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// ID �nico do usu�rio no sistema
    /// </summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usu�rio
    /// </summary>
    /// <example>Jo�o Silva Santos</example>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usu�rio
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usu�rio
    /// </summary>
    /// <example>joao_silva</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles/permiss�es do usu�rio (Admin, Manager, User)
    /// </summary>
    /// <example>["User"]</example>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Token JWT de acesso (v�lido por 1 hora)
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token de atualiza��o para renovar o JWT expirado (v�lido por 7 dias)
    /// </summary>
    /// <example>a1b2c3d4e5f67890abcdef1234567890...</example>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de expira��o do token JWT
    /// </summary>
    /// <example>2025-10-22T21:30:00Z</example>
    public DateTime TokenExpiresAt { get; set; }
}
