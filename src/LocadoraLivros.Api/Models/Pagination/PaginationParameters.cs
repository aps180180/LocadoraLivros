namespace LocadoraLivros.Api.Models.Pagination;

/// <summary>
/// Par�metros base para pagina��o de resultados
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// N�mero da p�gina (come�a em 1)
    /// </summary>
    /// <example>1</example>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por p�gina (m�ximo 100)
    /// </summary>
    /// <example>10</example>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Campo para ordena��o (opcional)
    /// </summary>
    /// <example>titulo</example>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Dire��o da ordena��o: asc ou desc (padr�o: asc)
    /// </summary>
    /// <example>asc</example>
    public string OrderDirection { get; set; } = "asc";

    /// <summary>
    /// Termo de busca/filtro (opcional)
    /// </summary>
    /// <example>senhor dos aneis</example>
    public string? SearchTerm { get; set; }
}
