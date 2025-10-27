using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IClienteService
{
    Task<PagedResult<Cliente>> GetAllAsync(PaginationParameters parameters);
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByCpfAsync(string cpf);
    Task<PagedResult<Cliente>> SearchAsync(string termo, PaginationParameters parameters);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task<bool> ExisteCPFAsync(string cpf, int? clienteId = null);
}
