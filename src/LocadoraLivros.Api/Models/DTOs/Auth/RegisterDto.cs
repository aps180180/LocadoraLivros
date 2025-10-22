namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Dados necessários para criar uma nova conta de usuário
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    /// <example>João Silva Santos</example>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail (será usado para login)
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usuário único (alfanumérico e underscore)
    /// </summary>
    /// <example>joao_silva</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Senha (mínimo 6 caracteres, deve conter maiúscula, minúscula, número e caractere especial)
    /// </summary>
    /// <example>Senha@123</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha (deve ser igual ao campo Password)
    /// </summary>
    /// <example>Senha@123</example>
    public string ConfirmPassword { get; set; } = string.Empty;
}
