namespace LocadoraLivros.Api.Models;

public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeTotal { get; set; }
    public decimal ValorDiaria { get; set; }
    public string? ImagemUrl { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }

    // Relacionamento N:M com Emprestimos através de EmprestimoItens
    public ICollection<EmprestimoItem> EmprestimoItens { get; set; } = new List<EmprestimoItem>();
}
