using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IEmprestimoService
{
    Task<IEnumerable<Emprestimo>> GetAllAsync();
    Task<Emprestimo?> GetByIdAsync(int id);
    Task<IEnumerable<Emprestimo>> GetAtivosAsync();
    Task<IEnumerable<Emprestimo>> GetAtrasadosAsync();
    Task<IEnumerable<Emprestimo>> GetByClienteIdAsync(int clienteId);
    Task<Emprestimo> RealizarEmprestimoAsync(int clienteId, List<(int livroId, int diasEmprestimo)> itens, string? observacoes = null);
    Task<Emprestimo> RealizarDevolucaoAsync(int emprestimoId);
    Task<Emprestimo> DevolverItemAsync(int emprestimoItemId);
    Task<decimal> CalcularValorTotalAsync(int emprestimoId);
}
