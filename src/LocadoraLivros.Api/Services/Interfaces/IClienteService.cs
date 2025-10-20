using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Cliente>> SearchAsync(string termo);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task<bool> ExisteCPFAsync(string cpf, int? clienteId = null);
}
