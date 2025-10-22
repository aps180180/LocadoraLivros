namespace LocadoraLivros.Api.Models.DTOs.User;

/// <summary>
/// Dados b�sicos de um usu�rio do sistema
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID �nico do usu�rio
    /// </summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usu�rio
    /// </summary>
    /// <example>Jo�o Silva Santos</example>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usu�rio
    /// </summary>
    /// <example>joao_silva</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usu�rio
    /// </summary>
    /// <example>joao.silva@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles/permiss�es do usu�rio
    /// </summary>
    /// <example>["User", "Manager"]</example>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Indica se o usu�rio est� ativo no sistema
    /// </summary>
    /// <example>true</example>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data e hora de cadastro do usu�rio
    /// </summary>
    /// <example>2025-10-21T20:30:00Z</example>
    public DateTime DataCadastro { get; set; }
}
