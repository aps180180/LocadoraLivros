using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmprestimoService> _logger;

    public EmprestimoService(ApplicationDbContext context, ILogger<EmprestimoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Emprestimo>> GetAllAsync()
    {
        return await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .OrderByDescending(e => e.DataEmprestimo)
            .ToListAsync();
    }

    public async Task<Emprestimo?> GetByIdAsync(int id)
    {
        return await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Emprestimo>> GetAtivosAsync()
    {
        return await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.Status == "Ativo")
            .OrderBy(e => e.DataPrevisaoDevolucao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Emprestimo>> GetAtrasadosAsync()
    {
        var hoje = DateTime.UtcNow.Date;

        return await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.Status == "Ativo" && e.DataPrevisaoDevolucao.Date < hoje)
            .OrderBy(e => e.DataPrevisaoDevolucao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Emprestimo>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Emprestimos
            .Include(e => e.Itens)
                .ThenInclude(i => i.Livro)
            .Where(e => e.ClienteId == clienteId)
            .OrderByDescending(e => e.DataEmprestimo)
            .ToListAsync();
    }

    public async Task<Emprestimo> RealizarEmprestimoAsync(
        int clienteId,
        List<(int livroId, int diasEmprestimo)> itens,
        string? observacoes = null)
    {
        // Validar cliente
        var cliente = await _context.Clientes.FindAsync(clienteId);
        if (cliente == null)
            throw new NotFoundException("Cliente", clienteId);

        if (!cliente.Ativo)
            throw new BusinessException("Cliente inativo não pode fazer empréstimos");

        if (itens == null || !itens.Any())
            throw new BusinessException("O empréstimo deve ter pelo menos um item");

        // Criar empréstimo
        var emprestimo = new Emprestimo
        {
            ClienteId = clienteId,
            DataEmprestimo = DateTime.UtcNow,
            Status = "Ativo",
            Observacoes = observacoes,
            ValorTotal = 0
        };

        var diasMaximo = 0;

        // Processar cada item
        foreach (var (livroId, diasEmprestimo) in itens)
        {
            var livro = await _context.Livros.FindAsync(livroId);

            if (livro == null)
                throw new NotFoundException("Livro", livroId);

            if (!livro.Ativo)
                throw new BusinessException($"O livro '{livro.Titulo}' está inativo");

            if (livro.QuantidadeDisponivel <= 0)
                throw new BusinessException($"O livro '{livro.Titulo}' não está disponível para empréstimo");

            // Decrementar quantidade disponível
            livro.QuantidadeDisponivel--;

            // Criar item do empréstimo
            var item = new EmprestimoItem
            {
                LivroId = livroId,
                DiasEmprestimo = diasEmprestimo,
                ValorDiaria = livro.ValorDiaria,
                ValorSubtotal = livro.ValorDiaria * diasEmprestimo
            };

            emprestimo.Itens.Add(item);
            emprestimo.ValorTotal += item.ValorSubtotal;

            if (diasEmprestimo > diasMaximo)
                diasMaximo = diasEmprestimo;
        }

        // Data de previsão de devolução baseada no maior prazo
        emprestimo.DataPrevisaoDevolucao = emprestimo.DataEmprestimo.AddDays(diasMaximo);

        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Empréstimo realizado: {EmprestimoId} - Cliente: {ClienteId} - {QtdItens} itens",
            emprestimo.Id, clienteId, emprestimo.Itens.Count);

        // Recarregar com navegações
        return (await GetByIdAsync(emprestimo.Id))!;
    }

    public async Task<Emprestimo> RealizarDevolucaoAsync(int emprestimoId)
    {
        var emprestimo = await GetByIdAsync(emprestimoId);

        if (emprestimo == null)
            throw new NotFoundException("Empréstimo", emprestimoId);

        if (emprestimo.Status != "Ativo")
            throw new BusinessException("Este empréstimo já foi devolvido");

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = "Devolvido";

        // Calcular multa se estiver atrasado
        if (emprestimo.DataDevolucao > emprestimo.DataPrevisaoDevolucao)
        {
            var diasAtraso = (emprestimo.DataDevolucao.Value.Date - emprestimo.DataPrevisaoDevolucao.Date).Days;
            emprestimo.ValorMulta = 0;

            // Calcular multa por item
            foreach (var item in emprestimo.Itens.Where(i => !i.DataDevolucaoItem.HasValue))
            {
                var multaItem = diasAtraso * item.ValorDiaria * 1.5m; // 50% de multa
                item.ValorMultaItem = multaItem;
                item.DataDevolucaoItem = emprestimo.DataDevolucao;
                emprestimo.ValorMulta += multaItem;
            }

            emprestimo.ValorTotal += emprestimo.ValorMulta.Value;
        }
        else
        {
            // Marcar todos os itens como devolvidos
            foreach (var item in emprestimo.Itens.Where(i => !i.DataDevolucaoItem.HasValue))
            {
                item.DataDevolucaoItem = emprestimo.DataDevolucao;
            }
        }

        // Incrementar quantidade disponível dos livros
        foreach (var item in emprestimo.Itens)
        {
            var livro = await _context.Livros.FindAsync(item.LivroId);
            if (livro != null)
            {
                livro.QuantidadeDisponivel++;
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Devolução realizada: {EmprestimoId} - Multa: {Multa}",
            emprestimoId, emprestimo.ValorMulta ?? 0);

        return emprestimo;
    }

    public async Task<Emprestimo> DevolverItemAsync(int emprestimoItemId)
    {
        var item = await _context.EmprestimoItens
            .Include(i => i.Emprestimo)
                .ThenInclude(e => e.Itens)
            .Include(i => i.Livro)
            .FirstOrDefaultAsync(i => i.Id == emprestimoItemId);

        if (item == null)
            throw new NotFoundException("Item de empréstimo", emprestimoItemId);

        if (item.DataDevolucaoItem.HasValue)
            throw new BusinessException("Este item já foi devolvido");

        var emprestimo = item.Emprestimo;

        if (emprestimo.Status != "Ativo")
            throw new BusinessException("Este empréstimo já foi finalizado");

        item.DataDevolucaoItem = DateTime.UtcNow;

        // Calcular multa se atrasado
        var dataLimite = emprestimo.DataEmprestimo.AddDays(item.DiasEmprestimo);
        if (item.DataDevolucaoItem > dataLimite)
        {
            var diasAtraso = (item.DataDevolucaoItem.Value.Date - dataLimite.Date).Days;
            item.ValorMultaItem = diasAtraso * item.ValorDiaria * 1.5m;
            emprestimo.ValorTotal += item.ValorMultaItem.Value;
            emprestimo.ValorMulta = (emprestimo.ValorMulta ?? 0) + item.ValorMultaItem.Value;
        }

        // Incrementar quantidade disponível
        item.Livro.QuantidadeDisponivel++;

        // Verificar se todos os itens foram devolvidos
        if (emprestimo.Itens.All(i => i.DataDevolucaoItem.HasValue))
        {
            emprestimo.Status = "Devolvido";
            emprestimo.DataDevolucao = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Item devolvido: {ItemId} - Empréstimo: {EmprestimoId}",
            emprestimoItemId, emprestimo.Id);

        return (await GetByIdAsync(emprestimo.Id))!;
    }

    public async Task<decimal> CalcularValorTotalAsync(int emprestimoId)
    {
        var emprestimo = await GetByIdAsync(emprestimoId);

        if (emprestimo == null)
            throw new NotFoundException("Empréstimo", emprestimoId);

        return emprestimo.ValorTotal;
    }
}
