using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; 

namespace LocadoraLivros.Api.Services;

public class LivroService : ILivroService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LivroService> _logger;

    public LivroService(ApplicationDbContext context, ILogger<LivroService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Models.Pagination.PagedResult<Livro>> GetAllAsync(PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.Ativo)
            .AsQueryable();

        // Aplicar busca se fornecida
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower().Trim();
            query = query.Where(l =>
                l.Titulo.ToLower().Contains(searchTerm) ||
                l.Autor.ToLower().Contains(searchTerm) ||
                l.ISBN.Contains(searchTerm));
        }

        // Aplicar ordenação
        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
    }

    public async Task<Livro?> GetByIdAsync(int id)
    {
        return await _context.Livros
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Models.Pagination.PagedResult<Livro>> GetDisponiveisAsync(PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.QuantidadeDisponivel > 0 && l.Ativo)
            .AsQueryable();

        // Aplicar busca se fornecida
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower().Trim();
            query = query.Where(l =>
                l.Titulo.ToLower().Contains(searchTerm) ||
                l.Autor.ToLower().Contains(searchTerm));
        }

        // Aplicar ordenação
        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
    }

    public async Task<Models.Pagination.PagedResult<Livro>> SearchAsync(string termo, PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.Ativo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            termo = termo.ToLower().Trim();
            query = query.Where(l =>
                l.Titulo.ToLower().Contains(termo) ||
                l.Autor.ToLower().Contains(termo) ||
                l.ISBN.Contains(termo) ||
                l.Categoria.ToLower().Contains(termo) ||
                l.Editora.ToLower().Contains(termo));
        }

        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
    }

    public async Task<Models.Pagination.PagedResult<Livro>> GetByCategoriaAsync(string categoria, PaginationParameters parameters)
    {
        var query = _context.Livros
            .Where(l => l.Ativo && l.Categoria.ToLower() == categoria.ToLower())
            .AsQueryable();

        query = ApplyOrdering(query, parameters);

        return await query.ToPagedResultAsync(parameters);
    }

    private IQueryable<Livro> ApplyOrdering(IQueryable<Livro> query, PaginationParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            return query.OrderBy(l => l.Titulo);
        }

        // Mapeamento de campos para ordenação
        var orderBy = parameters.OrderBy.ToLower() switch
        {
            "titulo" => "Titulo",
            "autor" => "Autor",
            "ano" => "AnoPublicacao",
            "categoria" => "Categoria",
            "valor" => "ValorDiaria",
            _ => "Titulo"
        };

        var direction = parameters.OrderDirection.ToLower() == "desc" ? "descending" : "ascending";

        return query.OrderBy($"{orderBy} {direction}");
    }

    

    public async Task<Livro> CreateAsync(Livro livro)
    {
        if (await ExisteISBNAsync(livro.ISBN))
            throw new BusinessException($"Já existe um livro cadastrado com o ISBN {livro.ISBN}");

        livro.DataCadastro = DateTime.UtcNow;
        livro.Ativo = true;

        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro criado: {LivroId} - {Titulo}", livro.Id, livro.Titulo);

        return livro;
    }

    public async Task<Livro> UpdateAsync(Livro livro)
    {
        var existente = await _context.Livros.FindAsync(livro.Id);

        if (existente == null)
            throw new NotFoundException("Livro", livro.Id);

        if (await ExisteISBNAsync(livro.ISBN, livro.Id))
            throw new BusinessException($"Já existe outro livro cadastrado com o ISBN {livro.ISBN}");

        existente.Titulo = livro.Titulo;
        existente.ISBN = livro.ISBN;
        existente.Autor = livro.Autor;
        existente.Editora = livro.Editora;
        existente.AnoPublicacao = livro.AnoPublicacao;
        existente.Categoria = livro.Categoria;
        existente.QuantidadeDisponivel = livro.QuantidadeDisponivel;
        existente.QuantidadeTotal = livro.QuantidadeTotal;
        existente.ValorDiaria = livro.ValorDiaria;
        existente.ImagemUrl = livro.ImagemUrl;
        existente.Ativo = livro.Ativo;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro atualizado: {LivroId} - {Titulo}", livro.Id, livro.Titulo);

        return existente;
    }

    public async Task DeleteAsync(int id)
    {
        var livro = await _context.Livros
            .Include(l => l.EmprestimoItens)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null)
            throw new NotFoundException("Livro", id);

        if (livro.EmprestimoItens.Any())
            throw new BusinessException("Não é possível excluir um livro que possui empréstimos registrados");

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro excluído: {LivroId} - {Titulo}", livro.Id, livro.Titulo);
    }

    public async Task<bool> ExisteISBNAsync(string isbn, int? livroId = null)
    {
        var query = _context.Livros.Where(l => l.ISBN == isbn);

        if (livroId.HasValue)
            query = query.Where(l => l.Id != livroId.Value);

        return await query.AnyAsync();
    }
}
