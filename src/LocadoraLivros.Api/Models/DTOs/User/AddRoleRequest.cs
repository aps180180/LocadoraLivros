using LocadoraLivros.Api.Shared.Constants;

namespace LocadoraLivros.Api.Models.DTOs.User;

/// <summary>
/// Solicita��o para adicionar uma role a um usu�rio
/// </summary>
public class AddRoleRequest
{
    /// <summary>
    /// Nome da role a ser adicionada (Admin, Manager ou User)
    /// </summary>
    /// <example>Manager</example>
    public string RoleName { get; set; } = string.Empty;
}
