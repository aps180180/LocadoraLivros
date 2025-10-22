namespace LocadoraLivros.Api.Models.DTOs.Emprestimo;

/// <summary>
/// Request para realizar um novo empr�stimo
/// </summary>
public class RealizarEmprestimoRequest
{
    /// <summary>
    /// ID do cliente que est� fazendo o empr�stimo
    /// </summary>
    /// <example>1</example>
    public int ClienteId { get; set; }

    /// <summary>
    /// Lista de livros a serem emprestados com suas respectivas dura��es
    /// </summary>
    public List<EmprestimoItemRequest> Itens { get; set; } = new();

    /// <summary>
    /// Observa��es ou notas sobre o empr�stimo (opcional)
    /// </summary>
    /// <example>Cliente solicitou extens�o de prazo</example>
    public string? Observacoes { get; set; }
}

/// <summary>
/// Representa um livro individual dentro do empr�stimo
/// </summary>
public class EmprestimoItemRequest
{
    /// <summary>
    /// ID do livro a ser emprestado
    /// </summary>
    /// <example>5</example>
    public int LivroId { get; set; }

    /// <summary>
    /// Quantidade de dias do empr�stimo para este livro (m�ximo 90 dias)
    /// </summary>
    /// <example>14</example>
    public int DiasEmprestimo { get; set; }
}
