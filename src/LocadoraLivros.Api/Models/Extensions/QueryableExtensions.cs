using LocadoraLivros.Api.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Extensions;

/// <summary>
/// Extension methods para facilitar paginação em queries
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Aplica paginação em uma query IQueryable
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
    /// Aplica paginação usando PaginationParameters
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParameters parameters)
    {
        return await query.ToPagedResultAsync(parameters.PageNumber, parameters.PageSize);
    }

    /// <summary>
    /// Aplica paginação de forma síncrona (use apenas se necessário)
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
