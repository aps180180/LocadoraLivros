namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Credenciais para autentica��o do usu�rio
/// </summary>
public class LoginDto
{
    /// <summary>
    /// E-mail do usu�rio cadastrado
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usu�rio
    /// </summary>
    /// <example>Senha@123</example>
    public string Password { get; set; } = string.Empty;
}
