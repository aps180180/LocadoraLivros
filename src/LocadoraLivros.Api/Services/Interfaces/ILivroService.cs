using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface ILivroService
{
    Task<PagedResult<Livro>> GetAllAsync(PaginationParameters parameters);
    Task<Livro?> GetByIdAsync(int id);
    Task<PagedResult<Livro>> GetDisponiveisAsync(PaginationParameters parameters);
    Task<PagedResult<Livro>> SearchAsync(string termo, PaginationParameters parameters);
    Task<PagedResult<Livro>> GetByCategoriaAsync(string categoria, PaginationParameters parameters);
    Task<Livro> CreateAsync(Livro livro);
    Task<Livro> UpdateAsync(Livro livro);
    Task DeleteAsync(int id);
    Task<bool> ExisteISBNAsync(string isbn, int? livroId = null);
}
