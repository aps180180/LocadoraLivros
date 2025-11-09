using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;

namespace LocadoraLivros.Api.Services.Interfaces.Emprestimo;

public interface IEmprestimoValidationService
{
    Task<(bool IsValid, string? ErrorMessage)> ValidateClienteAsync(int clienteId, ConfiguracaoEmprestimo config);
    Task<(bool IsValid, string? ErrorMessage)> ValidateLivrosAsync(List<EmprestimoItemRequest> itens);
    Task<(bool IsValid, string? ErrorMessage)> ValidatePrazosAsync(List<EmprestimoItemRequest> itens, ConfiguracaoEmprestimo config);
}
