namespace LocadoraLivros.Api.Models.DTOs.Livro;

/// <summary>
/// DTO para criação de livro
/// </summary>
public class CreateLivroDto
{
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string? Editora { get; set; }
    public int? AnoPublicacao { get; set; }
    public string? Categoria { get; set; }
    public int QuantidadeTotal { get; set; }
    public decimal ValorDiaria { get; set; }
}
