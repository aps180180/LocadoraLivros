namespace LocadoraLivros.Api.Models;

public class Emprestimo
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevisaoDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }

    public decimal ValorTotal { get; set; }
    public decimal? ValorMulta { get; set; }
    public string Status { get; set; } = string.Empty; // "Ativo", "Devolvido", "Atrasado"
    public string? Observacoes { get; set; }

    // Relacionamento N:M com Livros através de EmprestimoItens
    public ICollection<EmprestimoItem> Itens { get; set; } = new List<EmprestimoItem>();
}
