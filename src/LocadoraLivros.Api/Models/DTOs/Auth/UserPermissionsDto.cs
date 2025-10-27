namespace LocadoraLivros.Api.Models.DTOs.Auth;

/// <summary>
/// Informações detalhadas sobre permissões do usuário
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// ID único do usuário
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles atribuídas ao usuário
    /// </summary>
    /// <example>["Admin"]</example>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Lista de policies que o usuário satisfaz (baseado nas roles)
    /// </summary>
    /// <example>["AdminOnly", "AdminOrManager"]</example>
    public List<string> Policies { get; set; } = new();

    /// <summary>
    /// Permissões específicas calculadas
    /// </summary>
    public bool CanCreateBooks { get; set; }
    public bool CanEditBooks { get; set; }
    public bool CanDeleteBooks { get; set; }
    public bool CanDeleteClients { get; set; }
    public bool CanManageUsers { get; set; }
    public bool CanManageRoles { get; set; }
}
