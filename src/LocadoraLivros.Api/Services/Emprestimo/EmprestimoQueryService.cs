using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces.Emprestimo;
using LocadoraLivros.Api.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services.Emprestimo;

public class EmprestimoQueryService : IEmprestimoQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly EmprestimoMapperService _mapper;
    private readonly IEmprestimoCalculationService _calculationService;

    public EmprestimoQueryService(
        ApplicationDbContext context,
        EmprestimoMapperService mapper,
        IEmprestimoCalculationService calculationService)
    {
        _context = context;
        _mapper = mapper;
        _calculationService = calculationService;
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Cliente.Nome.ToLower().Contains(searchTerm) ||
                e.Cliente.CPF.Contains(searchTerm));
        }

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

        var dtos = pagedResult.Items.Select(e => _mapper.MapToDto(e)).ToList();

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

        return emprestimo == null ? null : _mapper.MapToDto(emprestimo);
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

        var dtos = pagedResult.Items.Select(e => _mapper.MapToDto(e)).ToList();

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

        var dtos = pagedResult.Items.Select(e => _mapper.MapToDto(e)).ToList();

        return new PagedResult<EmprestimoResponseDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<PagedResult<EmprestimoResponseDto>> GetByClienteIdAsync(
        int clienteId,
        PaginationParameters parameters)
    {
        var query = _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.ClienteId == clienteId);

        query = query.OrderByDescending(e => e.DataEmprestimo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(e => _mapper.MapToDto(e)).ToList();

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

        if (emprestimo.ValorMulta.HasValue)
        {
            valorTotal += emprestimo.ValorMulta.Value;
        }
        else if (emprestimo.Status == EmprestimoStatus.Ativo)
        {
            var hoje = DateTime.UtcNow;
            if (hoje > emprestimo.DataPrevisaoDevolucao)
            {
                var multaHipotetica = await _calculationService.CalcularMultaAsync(emprestimo, hoje);
                valorTotal += multaHipotetica;
            }
        }

        return valorTotal;
    }
}
