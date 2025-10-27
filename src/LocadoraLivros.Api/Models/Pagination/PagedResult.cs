namespace LocadoraLivros.Api.Models.Pagination;

/// <summary>
/// Resultado paginado contendo dados e metadados de pagina��o
/// </summary>
/// <typeparam name="T">Tipo dos itens na lista</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Lista de itens da p�gina atual
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Metadados de pagina��o (p�gina atual, total, etc)
    /// </summary>
    public PaginationMetadata Pagination { get; set; } = null!;

    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        Pagination = new PaginationMetadata(pageNumber, pageSize, totalCount);
    }
}
