namespace LocadoraLivros.Api.Models.DTOs.User;

/// <summary>
/// Solicita��o para remover uma role de um usu�rio
/// </summary>
public class RemoveRoleRequest
{
    /// <summary>
    /// Nome da role a ser removida (Admin, Manager ou User)
    /// </summary>
    /// <example>Manager</example>
    public string RoleName { get; set; } = string.Empty;
}
