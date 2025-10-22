namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Credenciais para autenticação do usuário
/// </summary>
public class LoginDto
{
    /// <summary>
    /// E-mail do usuário cadastrado
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    /// <example>Senha@123</example>
    public string Password { get; set; } = string.Empty;
}
