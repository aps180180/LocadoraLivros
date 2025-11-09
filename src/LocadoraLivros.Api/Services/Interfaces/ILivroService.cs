using LocadoraLivros.Api.Models.DTOs.Livro;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface ILivroService
{
    Task<PagedResult<LivroDto>> GetAllAsync(PaginationParameters parameters);
    Task<LivroDto?> GetByIdAsync(int id);
    Task<PagedResult<LivroDto>> GetDisponiveisAsync(PaginationParameters parameters);
    Task<PagedResult<LivroDto>> SearchAsync(string termo, PaginationParameters parameters);
    Task<PagedResult<LivroDto>> GetByCategoriaAsync(string categoria, PaginationParameters parameters);
    Task<(bool Success, string? Message, LivroDto? Data)> CreateAsync(CreateLivroDto dto);
    Task<(bool Success, string? Message, LivroDto? Data)> UpdateAsync(int id, UpdateLivroDto dto);
    Task<(bool Success, string? Message)> DeleteAsync(int id);
    
}
