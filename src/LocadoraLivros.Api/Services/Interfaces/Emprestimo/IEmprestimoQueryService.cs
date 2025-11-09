using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces.Emprestimo;

public interface IEmprestimoQueryService
{
    Task<PagedResult<EmprestimoResponseDto>> GetAllAsync(PaginationParameters parameters);
    Task<EmprestimoResponseDto?> GetByIdAsync(int id);
    Task<PagedResult<EmprestimoResponseDto>> GetAtivosAsync(PaginationParameters parameters);
    Task<PagedResult<EmprestimoResponseDto>> GetAtrasadosAsync(PaginationParameters parameters);
    Task<PagedResult<EmprestimoResponseDto>> GetByClienteIdAsync(int clienteId, PaginationParameters parameters);
    Task<decimal> CalcularValorTotalAsync(int id);
}
