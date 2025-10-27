namespace LocadoraLivros.Api.Models.DTOs.Emprestimo;

/// <summary>
/// DTO para retorno de empréstimo completo
/// </summary>
public class EmprestimoResponseDto
{
    public int Id { get; set; }

    // Dados do Cliente
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteCPF { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;

    // Datas
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevisaoDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }

    // Valores
    public decimal ValorTotal { get; set; }
    public decimal? ValorMulta { get; set; }

    // Status
    public string Status { get; set; } = string.Empty;
    public string? Observacoes { get; set; }

    // Itens do empréstimo
    public List<EmprestimoItemResponseDto> Itens { get; set; } = new();

    // Campos calculados
    public bool EstaAtrasado { get; set; }
    public int? DiasAtraso { get; set; }
}

/// <summary>
/// DTO para item do empréstimo
/// </summary>
public class EmprestimoItemResponseDto
{
    public int Id { get; set; }

    // Dados do Livro
    public int LivroId { get; set; }
    public string LivroTitulo { get; set; } = string.Empty;
    public string LivroAutor { get; set; } = string.Empty;
    public string LivroISBN { get; set; } = string.Empty;

    // Valores
    public int DiasEmprestimo { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal ValorSubtotal { get; set; }

    // Devolução
    public DateTime? DataDevolucaoItem { get; set; }
    public decimal? ValorMultaItem { get; set; }
}
