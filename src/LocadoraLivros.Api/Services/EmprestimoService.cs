using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LocadoraLivros.Api.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmprestimoService> _logger;

    public EmprestimoService(ApplicationDbContext context, ILogger<EmprestimoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string? Message, EmprestimoResponseDto? Data)> RealizarEmprestimoAsync(RealizarEmprestimoRequest request)
    {
        // ✅ INICIAR TRANSAÇÃO
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Validar cliente
            var cliente = await _context.Clientes.FindAsync(request.ClienteId);
            if (cliente == null)
                return (false, "Cliente não encontrado", null);

            if (!cliente.Ativo)
                return (false, "Cliente está inativo", null);

            // Validar livros
            var livrosIds = request.Itens.Select(x => x.LivroId).ToList();
            var livros = await _context.Livros
                .Where(l => livrosIds.Contains(l.Id))
                .ToListAsync();

            if (livros.Count != livrosIds.Count)
                return (false, "Um ou mais livros não foram encontrados", null);

            // Verificar disponibilidade
            foreach (var item in request.Itens)
            {
                var livro = livros.First(l => l.Id == item.LivroId);

                if (!livro.Ativo)
                    return (false, $"O livro '{livro.Titulo}' está inativo", null);

                if (livro.QuantidadeDisponivel < 1)
                    return (false, $"O livro '{livro.Titulo}' não está disponível", null);
            }

            // Criar empréstimo
            var emprestimo = new Emprestimo
            {
                ClienteId = request.ClienteId,
                DataEmprestimo = DateTime.UtcNow,
                Status = EmprestimoStatus.Ativo,
                Observacoes = request.Observacoes,
                ValorTotal = 0
            };

            // Adicionar itens
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

                // Decrementar quantidade disponível
                livro.QuantidadeDisponivel--;
            }

            // Calcular data de previsão (maior prazo dos itens)
            var maiorPrazo = request.Itens.Max(x => x.DiasEmprestimo);
            emprestimo.DataPrevisaoDevolucao = DateTime.UtcNow.AddDays(maiorPrazo);

            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();

            // COMMIT DA TRANSAÇÃO
            await transaction.CommitAsync();

            // Carregar relações para o DTO (após commit)
            await _context.Entry(emprestimo)
                .Reference(e => e.Cliente)
                .LoadAsync();

            await _context.Entry(emprestimo)
                .Collection(e => e.Itens)
                .LoadAsync();

            foreach (var item in emprestimo.Itens)
            {
                await _context.Entry(item)
                    .Reference(i => i.Livro)
                    .LoadAsync();
            }

            var dto = MapToDto(emprestimo);

            _logger.LogInformation("Empréstimo {EmprestimoId} criado com sucesso para cliente {ClienteId}",
                emprestimo.Id, cliente.Id);

            return (true, "Empréstimo realizado com sucesso", dto);
        }
        catch (Exception ex)
        {
            // ROLLBACK EM CASO DE ERRO
            await transaction.RollbackAsync();

            _logger.LogError(ex, "Erro ao realizar empréstimo para cliente {ClienteId}", request.ClienteId);

            return (false, "Erro ao processar empréstimo. Tente novamente.", null);
        }
    }

    public async Task<(bool Success, string? Message)> DevolverEmprestimoAsync(int id)
    {
        // USAR TRANSAÇÃO TAMBÉM NA DEVOLUÇÃO
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

            // Calcular multa se houver atraso
            if (dataDevolucao > emprestimo.DataPrevisaoDevolucao)
            {
                var diasAtraso = (dataDevolucao.Date - emprestimo.DataPrevisaoDevolucao.Date).Days;
                emprestimo.ValorMulta = emprestimo.ValorTotal * 0.5m * diasAtraso;
            }

            // Marcar todos os itens como devolvidos e incrementar estoque
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

            _logger.LogInformation("Empréstimo {EmprestimoId} devolvido com sucesso", id);

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
        //  USAR TRANSAÇÃO TAMBÉM NA DEVOLUÇÃO DE ITEM
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

            // Calcular multa do item se houver atraso
            if (dataDevolucao > item.Emprestimo.DataPrevisaoDevolucao)
            {
                var diasAtraso = (dataDevolucao.Date - item.Emprestimo.DataPrevisaoDevolucao.Date).Days;
                item.ValorMultaItem = item.ValorSubtotal * 0.5m * diasAtraso;
            }

            // Incrementar quantidade disponível
            item.Livro.QuantidadeDisponivel++;

            // Verificar se todos os itens foram devolvidos
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

            _logger.LogInformation("Item {ItemId} do empréstimo {EmprestimoId} devolvido",
                itemId, item.EmprestimoId);

            return (true, "Item devolvido com sucesso");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex, "Erro ao devolver item {ItemId}", itemId);

            return (false, "Erro ao processar devolução. Tente novamente.");
        }
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .AsQueryable();

        // Busca
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Cliente.Nome.ToLower().Contains(searchTerm) ||
                e.Cliente.CPF.Contains(searchTerm));
        }

        // Ordenação
        query = parameters.OrderBy?.ToLower() switch
        {
            "data" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(e => e.DataEmprestimo)
                : query.OrderBy(e => e.DataEmprestimo),
            "cliente" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(e => e.Cliente.Nome)
                : query.OrderBy(e => e.Cliente.Nome),
            "valor" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(e => e.ValorTotal)
                : query.OrderBy(e => e.ValorTotal),
            _ => query.OrderByDescending(e => e.DataEmprestimo)
        };

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<EmprestimoResponseDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<EmprestimoResponseDto?> GetByIdAsync(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

        return emprestimo == null ? null : MapToDto(emprestimo);
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetAtivosAsync(PaginationParameters parameters)
    {
        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.Status == EmprestimoStatus.Ativo);

        query = query.OrderByDescending(e => e.DataEmprestimo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<EmprestimoResponseDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetAtrasadosAsync(PaginationParameters parameters)
    {
        var hoje = DateTime.UtcNow;

        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.Status == EmprestimoStatus.Ativo && e.DataPrevisaoDevolucao < hoje);

        query = query.OrderBy(e => e.DataPrevisaoDevolucao);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<EmprestimoResponseDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetByClienteIdAsync(int clienteId, PaginationParameters parameters)
    {
        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.ClienteId == clienteId);

        query = query.OrderByDescending(e => e.DataEmprestimo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<EmprestimoResponseDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<decimal> CalcularValorTotalAsync(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Itens)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprestimo == null)
            return 0;

        var valorTotal = emprestimo.ValorTotal;

        // Se houver multa, adicionar
        if (emprestimo.ValorMulta.HasValue)
        {
            valorTotal += emprestimo.ValorMulta.Value;
        }
        // Se ainda não devolvido, calcular multa hipotética
        else if (emprestimo.Status == EmprestimoStatus.Ativo)
        {
            var hoje = DateTime.UtcNow;
            if (hoje > emprestimo.DataPrevisaoDevolucao)
            {
                var diasAtraso = (hoje.Date - emprestimo.DataPrevisaoDevolucao.Date).Days;
                var multaHipotetica = emprestimo.ValorTotal * 0.5m * diasAtraso;
                valorTotal += multaHipotetica;
            }
        }

        return valorTotal;
    }

    /// <summary>
    /// Mapeia Emprestimo para EmprestimoResponseDto
    /// </summary>
    private EmprestimoResponseDto MapToDto(Emprestimo emprestimo)
    {
        var now = DateTime.UtcNow;
        var estaAtrasado = emprestimo.Status == EmprestimoStatus.Ativo &&
                          emprestimo.DataPrevisaoDevolucao < now;

        int? diasAtraso = null;
        if (estaAtrasado)
        {
            diasAtraso = (now.Date - emprestimo.DataPrevisaoDevolucao.Date).Days;
        }

        return new EmprestimoResponseDto
        {
            Id = emprestimo.Id,

            // Cliente
            ClienteId = emprestimo.ClienteId,
            ClienteNome = emprestimo.Cliente?.Nome ?? "",
            ClienteCPF = emprestimo.Cliente?.CPF ?? "",
            ClienteEmail = emprestimo.Cliente?.Email ?? "",

            // Datas
            DataEmprestimo = emprestimo.DataEmprestimo,
            DataPrevisaoDevolucao = emprestimo.DataPrevisaoDevolucao,
            DataDevolucao = emprestimo.DataDevolucao,

            // Valores
            ValorTotal = emprestimo.ValorTotal,
            ValorMulta = emprestimo.ValorMulta,

            // Status
            Status = emprestimo.Status.ToString(),
            Observacoes = emprestimo.Observacoes,

            // Calculados
            EstaAtrasado = estaAtrasado,
            DiasAtraso = diasAtraso,

            // Itens
            Itens = emprestimo.Itens?.Select(item => new EmprestimoItemResponseDto
            {
                Id = item.Id,
                LivroId = item.LivroId,
                LivroTitulo = item.Livro?.Titulo ?? "",
                LivroAutor = item.Livro?.Autor ?? "",
                LivroISBN = item.Livro?.ISBN ?? "",
                DiasEmprestimo = item.DiasEmprestimo,
                ValorDiaria = item.ValorDiaria,
                ValorSubtotal = item.ValorSubtotal,
                DataDevolucaoItem = item.DataDevolucaoItem,
                ValorMultaItem = item.ValorMultaItem
            }).ToList() ?? new List<EmprestimoItemResponseDto>()
        };
    }
}
