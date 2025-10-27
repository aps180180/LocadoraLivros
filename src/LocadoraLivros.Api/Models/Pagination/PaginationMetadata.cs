namespace LocadoraLivros.Api.Models.Pagination;

/// <summary>
/// Metadados de paginação
/// </summary>
public class PaginationMetadata
{
    /// <summary>
    /// Página atual
    /// </summary>
    /// <example>1</example>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total de páginas disponíveis
    /// </summary>
    /// <example>5</example>
    public int TotalPages { get; set; }

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens em todos os registros
    /// </summary>
    /// <example>47</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// Indica se existe página anterior
    /// </summary>
    /// <example>false</example>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Indica se existe próxima página
    /// </summary>
    /// <example>true</example>
    public bool HasNext => CurrentPage < TotalPages;

    public PaginationMetadata(int currentPage, int pageSize, int totalCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
