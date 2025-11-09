using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Services.Interfaces.Emprestimo;
using LocadoraLivros.Api.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services.Emprestimo;

/// <summary>
/// Serviço principal de empréstimos - Orquestrador
/// </summary>
public class EmprestimoService : IEmprestimoService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguracaoService _configuracaoService;
    private readonly IEmprestimoValidationService _validationService;
    private readonly IEmprestimoCalculationService _calculationService;
    private readonly EmprestimoMapperService _mapper;
    private readonly IEmprestimoQueryService _queryService;
    private readonly ILogger<EmprestimoService> _logger;

    public EmprestimoService(
        ApplicationDbContext context,
        IConfiguracaoService configuracaoService,
        IEmprestimoValidationService validationService,
        IEmprestimoCalculationService calculationService,
        EmprestimoMapperService mapper,
        IEmprestimoQueryService queryService,
        ILogger<EmprestimoService> logger)
    {
        _context = context;
        _configuracaoService = configuracaoService;
        _validationService = validationService;
        _calculationService = calculationService;
        _mapper = mapper;
        _queryService = queryService;
        _logger = logger;
    }

    // ========== DELEGAÇÃO PARA QUERY SERVICE ==========

    public Task<PagedResult<EmprestimoResponseDto>> GetAllAsync(PaginationParameters parameters)
        => _queryService.GetAllAsync(parameters);

    public Task<EmprestimoResponseDto?> GetByIdAsync(int id)
        => _queryService.GetByIdAsync(id);

    public Task<PagedResult<EmprestimoResponseDto>> GetAtivosAsync(PaginationParameters parameters)
        => _queryService.GetAtivosAsync(parameters);

    public Task<PagedResult<EmprestimoResponseDto>> GetAtrasadosAsync(PaginationParameters parameters)
        => _queryService.GetAtrasadosAsync(parameters);

    public Task<PagedResult<EmprestimoResponseDto>> GetByClienteIdAsync(int clienteId, PaginationParameters parameters)
        => _queryService.GetByClienteIdAsync(clienteId, parameters);

    public Task<decimal> CalcularValorTotalAsync(int id)
        => _queryService.CalcularValorTotalAsync(id);

    // ========== OPERAÇÕES DE NEGÓCIO ==========

    public async Task<(bool Success, string? Message, EmprestimoResponseDto? Data)>
        RealizarEmprestimoAsync(RealizarEmprestimoRequest request)
    {
        var config = await _configuracaoService.GetConfiguracaoAtualAsync();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // ✅ VALIDAÇÕES (delegadas)
            var (clienteValido, mensagemCliente) = await _validationService.ValidateClienteAsync(
                request.ClienteId, config);

            if (!clienteValido)
                return (false, mensagemCliente, null);

            if (request.Itens.Count > config.MaximoLivrosPorEmprestimo)
            {
                return (false,
                    $"Máximo de {config.MaximoLivrosPorEmprestimo} livros por empréstimo",
                    null);
            }

            var (prazosValidos, mensagemPrazos) = await _validationService.ValidatePrazosAsync(
                request.Itens, config);

            if (!prazosValidos)
                return (false, mensagemPrazos, null);

            var (livrosValidos, mensagemLivros) = await _validationService.ValidateLivrosAsync(
                request.Itens);

            if (!livrosValidos)
                return (false, mensagemLivros, null);

            // ========== BUSCAR DADOS ==========
            var cliente = (await _context.Clientes.FindAsync(request.ClienteId))!;

            var livrosIds = request.Itens.Select(x => x.LivroId).ToList();
            var livros = await _context.Livros
                .Where(l => livrosIds.Contains(l.Id))
                .ToListAsync();

            // ========== CRIAR EMPRÉSTIMO ==========
            var emprestimo = new Models.Emprestimo
            {
                ClienteId = request.ClienteId,
                DataEmprestimo = DateTime.UtcNow,
                Status = EmprestimoStatus.Ativo,
                Observacoes = request.Observacoes,
                ValorTotal = 0
            };

            // ========== ADICIONAR ITENS ==========
            foreach (var itemRequest in request.Itens)
            {
                var livro = livros.First(l => l.Id == itemRequest.LivroId);

                var item = new EmprestimoItem
                {
                    LivroId = livro.Id,
                    DiasEmprestimo = itemRequest.DiasEmprestimo,
                    ValorDiaria = livro.ValorDiaria,
                    ValorSubtotal = livro.ValorDiaria * itemRequest.DiasEmprestimo
                };

                emprestimo.Itens.Add(item);
                emprestimo.ValorTotal += item.ValorSubtotal;
                livro.QuantidadeDisponivel--;
            }

            // APLICAR DESCONTO VIP (delegado)
            if (cliente.TipoCliente == TipoCliente.VIP && config.DescontoClienteVip > 0)
            {
                var desconto = _calculationService.CalcularDescontoVip(emprestimo.ValorTotal, config);
                emprestimo.ValorTotal -= desconto;
                emprestimo.Observacoes += $" [Desconto VIP de {config.DescontoClienteVip:P0}: {desconto:C}]";

                _logger.LogInformation(
                    "Desconto VIP aplicado: {Desconto:C} para cliente {ClienteId}",
                    desconto, cliente.Id);
            }

            //  CALCULAR PRAZO (delegado)
            var maiorPrazo = request.Itens.Max(x => x.DiasEmprestimo);
            var prazoFinal = _calculationService.CalcularPrazoComBonus(maiorPrazo, cliente, config);

            if (prazoFinal > maiorPrazo)
            {
                emprestimo.Observacoes += $" [+{prazoFinal - maiorPrazo} dias extras VIP]";

                _logger.LogInformation(
                    "Dias adicionais VIP: +{Dias} para cliente {ClienteId}",
                    prazoFinal - maiorPrazo, cliente.Id);
            }

            emprestimo.DataPrevisaoDevolucao = DateTime.UtcNow.AddDays(prazoFinal);

            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // ========== CARREGAR RELAÇÕES ==========
            await _context.Entry(emprestimo).Reference(e => e.Cliente).LoadAsync();
            await _context.Entry(emprestimo).Collection(e => e.Itens).LoadAsync();

            foreach (var item in emprestimo.Itens)
            {
                await _context.Entry(item).Reference(i => i.Livro).LoadAsync();
            }

            var dto = _mapper.MapToDto(emprestimo);

            _logger.LogInformation(
                "Empréstimo {EmprestimoId} criado. Cliente: {ClienteId}, Valor: {Valor:C}, Prazo: {Prazo} dias",
                emprestimo.Id, cliente.Id, emprestimo.ValorTotal, prazoFinal);

            return (true, "Empréstimo realizado com sucesso", dto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao realizar empréstimo para cliente {ClienteId}", request.ClienteId);
            return (false, "Erro ao processar empréstimo. Tente novamente.", null);
        }
    }

    public async Task<(bool Success, string? Message)> DevolverEmprestimoAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Itens)
                    .ThenInclude(i => i.Livro)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null)
                return (false, "Empréstimo não encontrado");

            if (emprestimo.Status == EmprestimoStatus.Devolvido)
                return (false, "Empréstimo já foi devolvido");

            var dataDevolucao = DateTime.UtcNow;
            emprestimo.DataDevolucao = dataDevolucao;

            // ✅ CALCULAR MULTA (delegado)
            emprestimo.ValorMulta = await _calculationService.CalcularMultaAsync(emprestimo, dataDevolucao);

            foreach (var item in emprestimo.Itens)
            {
                if (item.DataDevolucaoItem == null)
                {
                    item.DataDevolucaoItem = dataDevolucao;
                    item.Livro.QuantidadeDisponivel++;
                }
            }

            emprestimo.Status = EmprestimoStatus.Devolvido;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Empréstimo {EmprestimoId} devolvido. Multa: {Multa:C}",
                id, emprestimo.ValorMulta ?? 0);

            return (true, "Empréstimo devolvido com sucesso");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao devolver empréstimo {EmprestimoId}", id);
            return (false, "Erro ao processar devolução. Tente novamente.");
        }
    }

    public async Task<(bool Success, string? Message)> DevolverItemAsync(int itemId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var item = await _context.EmprestimoItens
                .Include(i => i.Emprestimo)
                .Include(i => i.Livro)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                return (false, "Item não encontrado");

            if (item.DataDevolucaoItem != null)
                return (false, "Item já foi devolvido");

            if (item.Emprestimo.Status == EmprestimoStatus.Devolvido)
                return (false, "Empréstimo já foi devolvido completamente");

            var dataDevolucao = DateTime.UtcNow;
            item.DataDevolucaoItem = dataDevolucao;

            // ✅ CALCULAR MULTA DO ITEM (usando calculation service)
            if (dataDevolucao > item.Emprestimo.DataPrevisaoDevolucao)
            {
                var config = await _configuracaoService.GetConfiguracaoAtualAsync();
                var diasAtraso = (dataDevolucao.Date - item.Emprestimo.DataPrevisaoDevolucao.Date).Days;
                var multaCalculada = item.ValorSubtotal * config.PercentualMultaDiaria * diasAtraso;

                var tetoItem = config.MultaMaxima * (item.ValorSubtotal / item.Emprestimo.ValorTotal);
                item.ValorMultaItem = Math.Min(multaCalculada, tetoItem);
            }

            item.Livro.QuantidadeDisponivel++;

            var todosDevolvidos = await _context.EmprestimoItens
                .Where(i => i.EmprestimoId == item.EmprestimoId)
                .AllAsync(i => i.DataDevolucaoItem != null);

            if (todosDevolvidos)
            {
                item.Emprestimo.Status = EmprestimoStatus.Devolvido;
                item.Emprestimo.DataDevolucao = dataDevolucao;
                item.Emprestimo.ValorMulta = await _context.EmprestimoItens
                    .Where(i => i.EmprestimoId == item.EmprestimoId)
                    .SumAsync(i => i.ValorMultaItem ?? 0);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Item {ItemId} do empréstimo {EmprestimoId} devolvido. Multa: {Multa:C}",
                itemId, item.EmprestimoId, item.ValorMultaItem ?? 0);

            return (true, "Item devolvido com sucesso");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao devolver item {ItemId}", itemId);
            return (false, "Erro ao processar devolução. Tente novamente.");
        }
    }
}
