namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Dados necess�rios para criar uma nova conta de usu�rio
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Nome completo do usu�rio
    /// </summary>
    /// <example>Jo�o Silva Santos</example>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Endere�o de e-mail (ser� usado para login)
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usu�rio �nico (alfanum�rico e underscore)
    /// </summary>
    /// <example>joao_silva</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Senha (m�nimo 6 caracteres, deve conter mai�scula, min�scula, n�mero e caractere especial)
    /// </summary>
    /// <example>Senha@123</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirma��o da senha (deve ser igual ao campo Password)
    /// </summary>
    /// <example>Senha@123</example>
    public string ConfirmPassword { get; set; } = string.Empty;
}
