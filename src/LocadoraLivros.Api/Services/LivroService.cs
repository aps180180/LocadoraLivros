using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Livro;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Services;

public class LivroService : ILivroService
{
    private readonly ApplicationDbContext _context;
    private readonly IStorageService _storageService;
    private readonly ILogger<LivroService> _logger;

    public LivroService(
        ApplicationDbContext context,
        IStorageService storageService,
        ILogger<LivroService> logger)
    {
        _context = context;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<PagedResult<LivroDto>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Livros.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(l =>
                l.Titulo.ToLower().Contains(searchTerm) ||
                l.Autor.ToLower().Contains(searchTerm) ||
                l.ISBN.Contains(searchTerm));
        }

        query = parameters.OrderBy?.ToLower() switch
        {
            "titulo" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(l => l.Titulo)
                : query.OrderBy(l => l.Titulo),
            "autor" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(l => l.Autor)
                : query.OrderBy(l => l.Autor),
            "ano" => parameters.OrderDirection.ToLower() == "desc"
                ? query.OrderByDescending(l => l.AnoPublicacao)
                : query.OrderBy(l => l.AnoPublicacao),
            _ => query.OrderBy(l => l.Titulo)
        };

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<LivroDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<LivroDto?> GetByIdAsync(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        return livro == null ? null : MapToDto(livro);
    }

    public async Task<PagedResult<LivroDto>> GetDisponiveisAsync(PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.Ativo && l.QuantidadeDisponivel > 0);

        query = query.OrderBy(l => l.Titulo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<LivroDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<PagedResult<LivroDto>> SearchAsync(string termo, PaginationParameters parameters)
    {
        var termoLower = termo.ToLower();

        var query = _context.Livros
            .Where(l => l.Titulo.ToLower().Contains(termoLower) ||
                       l.Autor.ToLower().Contains(termoLower) ||
                       l.ISBN.Contains(termo) ||
                       (l.Editora != null && l.Editora.ToLower().Contains(termoLower)) ||
                       (l.Categoria != null && l.Categoria.ToLower().Contains(termoLower)));

        query = query.OrderBy(l => l.Titulo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<LivroDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<PagedResult<LivroDto>> GetByCategoriaAsync(string categoria, PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.Categoria != null && l.Categoria.ToLower() == categoria.ToLower());

        query = query.OrderBy(l => l.Titulo);

        var pagedResult = await query.ToPagedResultAsync(parameters);

        var dtos = pagedResult.Items.Select(MapToDto).ToList();

        return new PagedResult<LivroDto>(
            dtos,
            pagedResult.Pagination.TotalCount,
            pagedResult.Pagination.CurrentPage,
            pagedResult.Pagination.PageSize);
    }

    public async Task<(bool Success, string? Message, LivroDto? Data)> CreateAsync(CreateLivroDto dto)
    {
        // Verificar se ISBN já existe
        if (await _context.Livros.AnyAsync(l => l.ISBN == dto.ISBN))
        {
            return (false, "ISBN já cadastrado", null);
        }

        var livro = new Livro
        {
            Titulo = dto.Titulo,
            ISBN = dto.ISBN,
            Autor = dto.Autor,
            Editora = dto.Editora!,
            AnoPublicacao = dto.AnoPublicacao,
            Categoria = dto.Categoria!,
            QuantidadeTotal = dto.QuantidadeTotal,
            QuantidadeDisponivel = dto.QuantidadeTotal,
            ValorDiaria = dto.ValorDiaria,
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro {LivroId} criado: {Titulo}", livro.Id, livro.Titulo);

        return (true, "Livro cadastrado com sucesso", MapToDto(livro));
    }

    public async Task<(bool Success, string? Message, LivroDto? Data)> UpdateAsync(int id, UpdateLivroDto dto)
    {
        var livro = await _context.Livros.FindAsync(id);

        if (livro == null)
            return (false, "Livro não encontrado", null);

        livro.Titulo = dto.Titulo;
        livro.Autor = dto.Autor;
        livro.Editora = dto.Editora!;
        livro.AnoPublicacao = dto.AnoPublicacao;
        livro.Categoria = dto.Categoria!;

        // Atualizar quantidade total e disponível
        var diferencaQuantidade = dto.QuantidadeTotal - livro.QuantidadeTotal;
        livro.QuantidadeTotal = dto.QuantidadeTotal;
        livro.QuantidadeDisponivel += diferencaQuantidade;

        // Garantir que disponível não fique negativo
        if (livro.QuantidadeDisponivel < 0)
            livro.QuantidadeDisponivel = 0;

        livro.ValorDiaria = dto.ValorDiaria;

        _context.Livros.Update(livro);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro {LivroId} atualizado: {Titulo}", livro.Id, livro.Titulo);

        return (true, "Livro atualizado com sucesso", MapToDto(livro));
    }

    public async Task<(bool Success, string? Message)> DeleteAsync(int id)
    {
        var livro = await _context.Livros.FindAsync(id);

        if (livro == null)
            return (false, "Livro não encontrado");

        // Verificar se há empréstimos ativos
        var possuiEmprestimosAtivos = await _context.EmprestimoItens
            .AnyAsync(ei => ei.LivroId == id && ei.DataDevolucaoItem == null);

        if (possuiEmprestimosAtivos)
        {
            return (false, "Não é possível excluir. Livro possui empréstimos ativos.");
        }

        // Soft delete
        livro.Ativo = false;
        _context.Livros.Update(livro);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro {LivroId} desativado: {Titulo}", livro.Id, livro.Titulo);

        return (true, "Livro excluído com sucesso");
    }

    
    // Método privado para mapear entidade para DTO
    private LivroDto MapToDto(Livro livro)
    {
        return new LivroDto
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            ISBN = livro.ISBN,
            Autor = livro.Autor,
            Editora = livro.Editora,
            AnoPublicacao = livro.AnoPublicacao,
            Categoria = livro.Categoria,
            QuantidadeTotal = livro.QuantidadeTotal,
            QuantidadeDisponivel = livro.QuantidadeDisponivel,
            ValorDiaria = livro.ValorDiaria,
            Ativo = livro.Ativo,
            DataCadastro = livro.DataCadastro
        };
    }
}
