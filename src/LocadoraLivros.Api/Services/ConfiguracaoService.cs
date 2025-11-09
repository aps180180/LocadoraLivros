using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services;

public class ConfiguracaoService : IConfiguracaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConfiguracaoService> _logger;

    // Cache simples em memória
    private static ConfiguracaoEmprestimo? _cache;
    private static DateTime? _cacheTime;
    private static readonly object _cacheLock = new();

    public ConfiguracaoService(
        ApplicationDbContext context,
        ILogger<ConfiguracaoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ConfiguracaoEmprestimo> GetConfiguracaoAtualAsync()
    {
        lock (_cacheLock)
        {
            // Cache válido por 5 minutos
            if (_cache != null && _cacheTime.HasValue &&
                (DateTime.UtcNow - _cacheTime.Value).TotalMinutes < 5)
            {
                _logger.LogDebug("Configuração retornada do cache");
                return _cache;
            }
        }

        var config = await _context.ConfiguracoesEmprestimo
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (config == null)
        {
            _logger.LogWarning("Configuração não encontrada. Criando padrão.");

            config = new ConfiguracaoEmprestimo
            {
                PercentualMultaDiaria = 0.5m,
                MultaMaxima = 1000.00m,
                MaximoLivrosPorEmprestimo = 5,
                MaximoEmprestimosAtivosCliente = 3,
                DiasMaximoEmprestimo = 30,
                DiasMinimosEmprestimo = 1,
                PermiteRenovacao = true,
                MaximoRenovacoes = 2,
                DiasRenovacao = 7,
                PermiteReserva = true,
                DiasValidadeReserva = 3,
                DiasAdicionaisClienteVip = 7,
                DescontoClienteVip = 0.1m,
                DiasAvisoVencimento = 2,
                DiasAtrasoParaBloqueio = 30,
                ValorDebitoParaBloqueio = 500.00m,
                DataCriacao = DateTime.UtcNow
            };

            _context.ConfiguracoesEmprestimo.Add(config);
            await _context.SaveChangesAsync();
        }

        lock (_cacheLock)
        {
            _cache = config;
            _cacheTime = DateTime.UtcNow;
        }

        _logger.LogInformation("Configuração carregada do banco de dados");
        return config;
    }

    public async Task<ConfiguracaoEmprestimo> AtualizarConfiguracaoAsync(
        ConfiguracaoEmprestimo config,
        string? usuarioId = null)
    {
        config.DataAtualizacao = DateTime.UtcNow;
        config.UsuarioAtualizacao = usuarioId;

        _context.ConfiguracoesEmprestimo.Update(config);
        await _context.SaveChangesAsync();

        LimparCache();

        _logger.LogInformation(
            "Configuração atualizada por usuário {UserId}",
            usuarioId ?? "Sistema");

        return config;
    }

    public void LimparCache()
    {
        lock (_cacheLock)
        {
            _cache = null;
            _cacheTime = null;
        }

        _logger.LogDebug("Cache de configuração limpo");
    }
}
