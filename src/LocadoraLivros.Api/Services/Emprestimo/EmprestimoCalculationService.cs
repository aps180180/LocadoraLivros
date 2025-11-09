using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Services.Interfaces.Emprestimo;
using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Services.Emprestimo;

public class EmprestimoCalculationService : IEmprestimoCalculationService
{
    private readonly IConfiguracaoService _configuracaoService;
    private readonly ILogger<EmprestimoCalculationService> _logger;

    public EmprestimoCalculationService(
        IConfiguracaoService configuracaoService,
        ILogger<EmprestimoCalculationService> logger)
    {
        _configuracaoService = configuracaoService;
        _logger = logger;
    }

    public async Task<decimal> CalcularMultaAsync(
        Models.Emprestimo emprestimo,
        DateTime dataDevolucao)
    {
        var config = await _configuracaoService.GetConfiguracaoAtualAsync();

        if (dataDevolucao <= emprestimo.DataPrevisaoDevolucao)
            return 0;

        var diasAtraso = (dataDevolucao.Date - emprestimo.DataPrevisaoDevolucao.Date).Days;
        var multa = emprestimo.ValorTotal * config.PercentualMultaDiaria * diasAtraso;

        if (multa > config.MultaMaxima)
        {
            _logger.LogWarning(
                "Multa calculada ({MultaCalculada:C}) excede teto ({Teto:C}). " +
                "Aplicando teto para empréstimo {EmprestimoId}",
                multa, config.MultaMaxima, emprestimo.Id);

            multa = config.MultaMaxima;
        }

        return multa;
    }

    public decimal CalcularDescontoVip(decimal valorTotal, ConfiguracaoEmprestimo config)
    {
        return valorTotal * config.DescontoClienteVip;
    }

    public int CalcularPrazoComBonus(int prazoBase, Cliente cliente, ConfiguracaoEmprestimo config)
    {
        if (cliente.TipoCliente == TipoCliente.VIP && config.DiasAdicionaisClienteVip > 0)
        {
            return prazoBase + config.DiasAdicionaisClienteVip;
        }

        return prazoBase;
    }
}
