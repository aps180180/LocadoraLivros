namespace LocadoraLivros.Api.Shared.Enums;

/// <summary>
/// Tipos de cliente do sistema
/// </summary>
public enum TipoCliente
{
    /// <summary>
    /// Cliente padrão
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Cliente VIP (descontos e benefícios)
    /// </summary>
    VIP = 1,

    /// <summary>
    /// Cliente corporativo
    /// </summary>
    Corporativo = 2
}
