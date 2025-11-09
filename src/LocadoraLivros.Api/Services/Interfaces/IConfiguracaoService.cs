using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IConfiguracaoService
{
    /// <summary>
    /// Obtém a configuração atual (com cache)
    /// </summary>
    Task<ConfiguracaoEmprestimo> GetConfiguracaoAtualAsync();

    /// <summary>
    /// Atualiza a configuração
    /// </summary>
    Task<ConfiguracaoEmprestimo> AtualizarConfiguracaoAsync(
        ConfiguracaoEmprestimo config,
        string? usuarioId = null);

    /// <summary>
    /// Limpa o cache de configuração
    /// </summary>
    void LimparCache();
}
