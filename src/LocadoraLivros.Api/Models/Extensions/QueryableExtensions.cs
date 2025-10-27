using LocadoraLivros.Api.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Extensions;

/// <summary>
/// Extension methods para facilitar pagina��o em queries
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Aplica pagina��o em uma query IQueryable
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Aplica pagina��o usando PaginationParameters
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParameters parameters)
    {
        return await query.ToPagedResultAsync(parameters.PageNumber, parameters.PageSize);
    }

    /// <summary>
    /// Aplica pagina��o de forma s�ncrona (use apenas se necess�rio)
    /// </summary>
    public static PagedResult<T> ToPagedResult<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = query.Count();

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}
