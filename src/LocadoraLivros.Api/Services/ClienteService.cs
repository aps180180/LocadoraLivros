using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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

    public async Task<Models.Pagination.PagedResult<Cliente>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Clientes
            .Where(c => c.Ativo)
            .AsQueryable();

        // Aplicar busca se fornecida
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower().Trim();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(searchTerm) ||
                c.CPF.Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm));
        }

        // Aplicar ordenação
        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
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

    public async Task<Models.Pagination.PagedResult<Cliente>> SearchAsync(string termo, PaginationParameters parameters)
    {
        var query = _context.Clientes
            .Where(c => c.Ativo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            termo = termo.ToLower().Trim();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(termo) ||
                c.CPF.Contains(termo) ||
                c.Email.ToLower().Contains(termo) ||
                c.Telefone.Contains(termo) ||
                c.Cidade.ToLower().Contains(termo));
        }

        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
    }

    private IQueryable<Cliente> ApplyOrdering(IQueryable<Cliente> query, PaginationParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            return query.OrderBy(c => c.Nome);
        }

        var orderBy = parameters.OrderBy.ToLower() switch
        {
            "nome" => "Nome",
            "cpf" => "CPF",
            "email" => "Email",
            "cidade" => "Cidade",
            "datacadastro" => "DataCadastro",
            _ => "Nome"
        };

        var direction = parameters.OrderDirection.ToLower() == "desc" ? "descending" : "ascending";

        return query.OrderBy($"{orderBy} {direction}");
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "").Trim();

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

        cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "").Trim();

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
