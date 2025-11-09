using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Services.Emprestimo;

public class EmprestimoMapperService
{
    public EmprestimoResponseDto MapToDto(Models.Emprestimo emprestimo)
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
            ClienteId = emprestimo.ClienteId,
            ClienteNome = emprestimo.Cliente?.Nome ?? "",
            ClienteCPF = emprestimo.Cliente?.CPF ?? "",
            ClienteEmail = emprestimo.Cliente?.Email ?? "",
            DataEmprestimo = emprestimo.DataEmprestimo,
            DataPrevisaoDevolucao = emprestimo.DataPrevisaoDevolucao,
            DataDevolucao = emprestimo.DataDevolucao,
            ValorTotal = emprestimo.ValorTotal,
            ValorMulta = emprestimo.ValorMulta,
            Status = emprestimo.Status.ToString(),
            Observacoes = emprestimo.Observacoes,
            EstaAtrasado = estaAtrasado,
            DiasAtraso = diasAtraso,
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
