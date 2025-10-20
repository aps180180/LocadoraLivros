using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services;

public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(ApplicationDbContext context, ILogger<ClienteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> GetByCpfAsync(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();

        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.CPF == cpf);
    }

    public async Task<IEnumerable<Cliente>> SearchAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await GetAllAsync();

        termo = termo.ToLower().Trim();

        return await _context.Clientes
            .Where(c => c.Ativo && (
                c.Nome.ToLower().Contains(termo) ||
                c.CPF.Contains(termo) ||
                c.Email.ToLower().Contains(termo) ||
                c.Telefone.Contains(termo)))
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        // Normalizar CPF
        cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "").Trim();

        // Validar CPF duplicado
        if (await ExisteCPFAsync(cliente.CPF))
            throw new BusinessException($"Já existe um cliente cadastrado com o CPF {cliente.CPF}");

        cliente.DataCadastro = DateTime.UtcNow;
        cliente.Ativo = true;

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente criado: {ClienteId} - {Nome}", cliente.Id, cliente.Nome);

        return cliente;
    }

    public async Task<Cliente> UpdateAsync(Cliente cliente)
    {
        var existente = await _context.Clientes.FindAsync(cliente.Id);

        if (existente == null)
            throw new NotFoundException("Cliente", cliente.Id);

        // Normalizar CPF
        cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "").Trim();

        // Validar CPF duplicado
        if (await ExisteCPFAsync(cliente.CPF, cliente.Id))
            throw new BusinessException($"Já existe outro cliente cadastrado com o CPF {cliente.CPF}");

        existente.Nome = cliente.Nome;
        existente.CPF = cliente.CPF;
        existente.Email = cliente.Email;
        existente.Telefone = cliente.Telefone;
        existente.Celular = cliente.Celular;
        existente.Endereco = cliente.Endereco;
        existente.Cidade = cliente.Cidade;
        existente.Estado = cliente.Estado;
        existente.CEP = cliente.CEP;
        existente.Ativo = cliente.Ativo;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente atualizado: {ClienteId} - {Nome}", cliente.Id, cliente.Nome);

        return existente;
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Emprestimos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
            throw new NotFoundException("Cliente", id);

        // Verificar se tem empréstimos
        if (cliente.Emprestimos.Any())
            throw new BusinessException("Não é possível excluir um cliente que possui empréstimos registrados");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente excluído: {ClienteId} - {Nome}", cliente.Id, cliente.Nome);
    }

    public async Task<bool> ExisteCPFAsync(string cpf, int? clienteId = null)
    {
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();

        var query = _context.Clientes.Where(c => c.CPF == cpf);

        if (clienteId.HasValue)
            query = query.Where(c => c.Id != clienteId.Value);

        return await query.AnyAsync();
    }
}
