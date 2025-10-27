namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Informa��es detalhadas sobre permiss�es do usu�rio
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// ID �nico do usu�rio
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usu�rio
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usu�rio
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usu�rio
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles atribu�das ao usu�rio
    /// </summary>
    /// <example>["Admin"]</example>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Lista de policies que o usu�rio satisfaz (baseado nas roles)
    /// </summary>
    /// <example>["AdminOnly", "AdminOrManager"]</example>
    public List<string> Policies { get; set; } = new();

    /// <summary>
    /// Permiss�es espec�ficas calculadas
    /// </summary>
    public bool CanCreateBooks { get; set; }
    public bool CanEditBooks { get; set; }
    public bool CanDeleteBooks { get; set; }
    public bool CanDeleteClients { get; set; }
    public bool CanManageUsers { get; set; }
    public bool CanManageRoles { get; set; }
}
