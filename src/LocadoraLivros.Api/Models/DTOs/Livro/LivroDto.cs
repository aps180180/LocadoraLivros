namespace LocadoraLivros.Api.Models.DTOs.Livro;

/// <summary>
/// DTO para retorno de livro
/// </summary>
public class LivroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string? Editora { get; set; }
    public int? AnoPublicacao { get; set; }
    public string? Categoria { get; set; }
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public decimal ValorDiaria { get; set; }
    public string? CapaUrl { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }

    // Campos calculados
    public bool Disponivel => QuantidadeDisponivel > 0;
    public int QuantidadeEmprestada => QuantidadeTotal - QuantidadeDisponivel;
}
