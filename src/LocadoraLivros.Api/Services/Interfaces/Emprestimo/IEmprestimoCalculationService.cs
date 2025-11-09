using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Services.Interfaces.Emprestimo;

public interface IEmprestimoCalculationService
{
    Task<decimal> CalcularMultaAsync(Models.Emprestimo emprestimo, DateTime dataDevolucao);
    decimal CalcularDescontoVip(decimal valorTotal, ConfiguracaoEmprestimo config);
    int CalcularPrazoComBonus(int prazoBase, Cliente cliente, ConfiguracaoEmprestimo config);
}
