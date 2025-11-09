using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Services.Interfaces.Emprestimo;
using LocadoraLivros.Api.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services.Emprestimo;

public class EmprestimoValidationService : IEmprestimoValidationService
{
    private readonly ApplicationDbContext _context;

    public EmprestimoValidationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool IsValid, string? ErrorMessage)> ValidateClienteAsync(
        int clienteId,
        ConfiguracaoEmprestimo config)
    {
        var cliente = await _context.Clientes.FindAsync(clienteId);

        if (cliente == null)
            return (false, "Cliente não encontrado");

        if (!cliente.Ativo)
            return (false, "Cliente está inativo");

        // Validar empréstimos ativos
        var emprestimosAtivos = await _context.Emprestimos
            .CountAsync(e => e.ClienteId == clienteId && e.Status == EmprestimoStatus.Ativo);

        if (emprestimosAtivos >= config.MaximoEmprestimosAtivosCliente)
        {
            return (false,
                $"Cliente já possui {emprestimosAtivos} empréstimos ativos. " +
                $"Máximo permitido: {config.MaximoEmprestimosAtivosCliente}");
        }

        return (true, null);
    }

    public async Task<(bool IsValid, string? ErrorMessage)> ValidateLivrosAsync(
        List<EmprestimoItemRequest> itens)
    {
        var livrosIds = itens.Select(x => x.LivroId).ToList();
        var livros = await _context.Livros
            .Where(l => livrosIds.Contains(l.Id))
            .ToListAsync();

        if (livros.Count != livrosIds.Count)
            return (false, "Um ou mais livros não foram encontrados");

        foreach (var item in itens)
        {
            var livro = livros.First(l => l.Id == item.LivroId);

            if (!livro.Ativo)
                return (false, $"O livro '{livro.Titulo}' está inativo");

            if (livro.QuantidadeDisponivel < 1)
                return (false, $"O livro '{livro.Titulo}' não está disponível");
        }

        return (true, null);
    }

    public Task<(bool IsValid, string? ErrorMessage)> ValidatePrazosAsync(
        List<EmprestimoItemRequest> itens,
        ConfiguracaoEmprestimo config)
    {
        foreach (var item in itens)
        {
            if (item.DiasEmprestimo < config.DiasMinimosEmprestimo)
            {
                return Task.FromResult<(bool, string?)>((false,
                    $"Prazo mínimo de empréstimo: {config.DiasMinimosEmprestimo} dias"));
            }

            if (item.DiasEmprestimo > config.DiasMaximoEmprestimo)
            {
                return Task.FromResult<(bool, string?)>((false,
                    $"Prazo máximo de empréstimo: {config.DiasMaximoEmprestimo} dias"));
            }
        }

        return Task.FromResult<(bool, string?)>((true, null));
    }
}
