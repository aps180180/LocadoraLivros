namespace LocadoraLivros.Api.Models;

public class EmprestimoItem
{
    public int Id { get; set; }

    // Relacionamento com Emprestimo
    public int EmprestimoId { get; set; }
    public Emprestimo Emprestimo { get; set; } = null!;

    // Relacionamento com Livro
    public int LivroId { get; set; }
    public Livro Livro { get; set; } = null!;

    // Dados específicos deste item
    public int DiasEmprestimo { get; set; }
    public decimal ValorDiaria { get; set; } // Valor da diária no momento do empréstimo
    public decimal ValorSubtotal { get; set; } // ValorDiaria * DiasEmprestimo
    public DateTime? DataDevolucaoItem { get; set; } // Permite devolver livros separadamente
    public decimal? ValorMultaItem { get; set; }
}
