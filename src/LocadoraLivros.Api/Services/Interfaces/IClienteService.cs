using LocadoraLivros.Api.Models.DTOs.Cliente;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IClienteService
{
    Task<PagedResult<ClienteDto>> GetAllAsync(PaginationParameters parameters);
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto?> GetByCpfAsync(string cpf);
    Task<PagedResult<ClienteDto>> SearchAsync(string termo, PaginationParameters parameters);
    Task<(bool Success, string? Message, ClienteDto? Data)> CreateAsync(CreateClienteDto dto);
    Task<(bool Success, string? Message, ClienteDto? Data)> UpdateAsync(int id, UpdateClienteDto dto);
    Task<(bool Success, string? Message)> DeleteAsync(int id);
}
