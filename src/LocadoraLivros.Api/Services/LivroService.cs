using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<Livro>> GetAllAsync()
    {
        return await _context.Livros
            .Where(l => l.Ativo)
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<Livro?> GetByIdAsync(int id)
    {
        return await _context.Livros
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Livro>> GetDisponiveisAsync()
    {
        return await _context.Livros
            .Where(l => l.QuantidadeDisponivel > 0 && l.Ativo)
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Livro>> SearchAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await GetAllAsync();

        termo = termo.ToLower().Trim();

        return await _context.Livros
            .Where(l => l.Ativo && (
                l.Titulo.ToLower().Contains(termo) ||
                l.Autor.ToLower().Contains(termo) ||
                l.ISBN.Contains(termo) ||
                l.Categoria.ToLower().Contains(termo) ||
                l.Editora.ToLower().Contains(termo)))
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Livro>> GetByCategoriaAsync(string categoria)
    {
        return await _context.Livros
            .Where(l => l.Ativo && l.Categoria.ToLower() == categoria.ToLower())
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<Livro> CreateAsync(Livro livro)
    {
        // Validar ISBN duplicado
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

        // Validar ISBN duplicado
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

        // Verificar se tem empréstimos
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
