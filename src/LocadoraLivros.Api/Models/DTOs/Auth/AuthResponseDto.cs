namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Resposta de autenticação contendo dados do usuário e tokens JWT
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// ID único do usuário no sistema
    /// </summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    /// <example>João Silva Santos</example>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usuário
    /// </summary>
    /// <example>joao_silva</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles/permissões do usuário (Admin, Manager, User)
    /// </summary>
    /// <example>["User"]</example>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Token JWT de acesso (válido por 1 hora)
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token de atualização para renovar o JWT expirado (válido por 7 dias)
    /// </summary>
    /// <example>a1b2c3d4e5f67890abcdef1234567890...</example>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de expiração do token JWT
    /// </summary>
    /// <example>2025-10-22T21:30:00Z</example>
    public DateTime TokenExpiresAt { get; set; }
}
