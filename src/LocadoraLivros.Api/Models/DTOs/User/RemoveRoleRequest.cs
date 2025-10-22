namespace LocadoraLivros.Api.Models.DTOs.User;

/// <summary>
/// Solicitação para remover uma role de um usuário
/// </summary>
public class RemoveRoleRequest
{
    /// <summary>
    /// Nome da role a ser removida (Admin, Manager ou User)
    /// </summary>
    /// <example>Manager</example>
    public string RoleName { get; set; } = string.Empty;
}
