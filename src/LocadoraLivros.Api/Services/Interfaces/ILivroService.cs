using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface ILivroService
{
    Task<IEnumerable<Livro>> GetAllAsync();
    Task<Livro?> GetByIdAsync(int id);
    Task<IEnumerable<Livro>> GetDisponiveisAsync();
    Task<IEnumerable<Livro>> SearchAsync(string termo);
    Task<IEnumerable<Livro>> GetByCategoriaAsync(string categoria);
    Task<Livro> CreateAsync(Livro livro);
    Task<Livro> UpdateAsync(Livro livro);
    Task DeleteAsync(int id);
    Task<bool> ExisteISBNAsync(string isbn, int? livroId = null);
}
