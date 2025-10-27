using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IEmprestimoService
{
    Task<(bool Success, string? Message, EmprestimoResponseDto? Data)> RealizarEmprestimoAsync(RealizarEmprestimoRequest request);
    Task<PagedResult<EmprestimoResponseDto>> GetAllAsync(PaginationParameters parameters);
    Task<EmprestimoResponseDto?> GetByIdAsync(int id);
    Task<PagedResult<EmprestimoResponseDto>> GetAtivosAsync(PaginationParameters parameters);
    Task<PagedResult<EmprestimoResponseDto>> GetAtrasadosAsync(PaginationParameters parameters);
    Task<PagedResult<EmprestimoResponseDto>> GetByClienteIdAsync(int clienteId, PaginationParameters parameters);
    Task<(bool Success, string? Message)> DevolverEmprestimoAsync(int id);
    Task<(bool Success, string? Message)> DevolverItemAsync(int itemId);
    Task<decimal> CalcularValorTotalAsync(int id);
}
