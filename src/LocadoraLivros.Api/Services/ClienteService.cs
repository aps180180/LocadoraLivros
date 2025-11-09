using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Cliente;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Enums;
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

    public async Task<PagedResult<ClienteDto>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(searchTerm) ||
                c.CPF.Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm));
        }

        query = parameters.OrderBy?.ToLower() switch
        {
            "nome" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Nome)
                : query.OrderBy(c => c.Nome),
            "data" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(c => c.DataCadastro)
                : query.OrderBy(c => c.DataCadastro),
            _ => query.OrderBy(c => c.Nome)
        };

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<ClienteDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        return cliente == null ? null : MapToDto(cliente);
    }

    public async Task<ClienteDto?> GetByCpfAsync(string cpf)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.CPF == cpf);
        return cliente == null ? null : MapToDto(cliente);
    }

    public async Task<PagedResult<ClienteDto>> SearchAsync(string termo, PaginationParameters parameters)
    {
        var termoLower = termo.ToLower();

        var query = _context.Clientes
            .Where(c => c.Nome.ToLower().Contains(termoLower) ||
                       c.CPF.Contains(termo) ||
                       c.Email.ToLower().Contains(termoLower) ||
                       (c.Cidade != null && c.Cidade.ToLower().Contains(termoLower)));

        query = query.OrderBy(c => c.Nome);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<ClienteDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<(bool Success, string? Message, ClienteDto? Data)> CreateAsync(CreateClienteDto dto)
    {
        // Verificar se CPF já existe
        if (await _context.Clientes.AnyAsync(c => c.CPF == dto.CPF))
        {
            return (false, "CPF já cadastrado", null);
        }

        // Verificar se email já existe
        if (await _context.Clientes.AnyAsync(c => c.Email == dto.Email))
        {
            return (false, "Email já cadastrado", null);
        }

        var cliente = new Cliente
        {
            Nome = dto.Nome,
            CPF = dto.CPF,
            Email = dto.Email,
            Telefone = dto.Telefone!,
            Celular = dto.Celular!,
            Endereco = dto.Endereco!,
            Cidade = dto.Cidade!,
            Estado = dto.Estado!,
            CEP = dto.CEP!,
            TipoCliente = dto.TipoCliente!,
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente {ClienteId} criado: {Nome}", cliente.Id, cliente.Nome);

        return (true, "Cliente cadastrado com sucesso", MapToDto(cliente));
    }

    public async Task<(bool Success, string? Message, ClienteDto? Data)> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return (false, "Cliente não encontrado", null);

        // Verificar se email já existe em outro cliente
        if (await _context.Clientes.AnyAsync(c => c.Email == dto.Email && c.Id != id))
        {
            return (false, "Email já cadastrado para outro cliente", null);
        }

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone!;
        cliente.Celular = dto.Celular;
        cliente.Endereco = dto.Endereco!;
        cliente.Cidade = dto.Cidade!;
        cliente.Estado = dto.Estado!;
        cliente.CEP = dto.CEP!;
        cliente.TipoCliente = dto.TipoCliente;

        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente {ClienteId} atualizado: {Nome}", cliente.Id, cliente.Nome);

        return (true, "Cliente atualizado com sucesso", MapToDto(cliente));
    }

    public async Task<(bool Success, string? Message)> DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return (false, "Cliente não encontrado");

        // Verificar se há empréstimos ativos
        var possuiEmprestimosAtivos = await _context.Emprestimos
            .AnyAsync(e => e.ClienteId == id && e.Status == EmprestimoStatus.Ativo);

        if (possuiEmprestimosAtivos)
        {
            return (false, "Não é possível excluir. Cliente possui empréstimos ativos.");
        }

        // Soft delete
        cliente.Ativo = false;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente {ClienteId} desativado: {Nome}", cliente.Id, cliente.Nome);

        return (true, "Cliente excluído com sucesso");
    }

    // Método privado para mapear entidade para DTO
    private ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Celular = cliente.Celular,
            Endereco = cliente.Endereco,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            CEP = cliente.CEP,
            TipoCliente = cliente.TipoCliente,
            TipoClienteDescricao = cliente.TipoCliente.ToString(),
            DataCadastro = cliente.DataCadastro,
            Ativo = cliente.Ativo
        };
    }
}
