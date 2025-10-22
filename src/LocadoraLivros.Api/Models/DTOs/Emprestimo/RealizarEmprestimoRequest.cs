namespace LocadoraLivros.Api.Models.DTOs.Emprestimo;

/// <summary>
/// Request para realizar um novo empréstimo
/// </summary>
public class RealizarEmprestimoRequest
{
    /// <summary>
    /// ID do cliente que está fazendo o empréstimo
    /// </summary>
    /// <example>1</example>
    public int ClienteId { get; set; }

    /// <summary>
    /// Lista de livros a serem emprestados com suas respectivas durações
    /// </summary>
    public List<EmprestimoItemRequest> Itens { get; set; } = new();

    /// <summary>
    /// Observações ou notas sobre o empréstimo (opcional)
    /// </summary>
    /// <example>Cliente solicitou extensão de prazo</example>
    public string? Observacoes { get; set; }
}

/// <summary>
/// Representa um livro individual dentro do empréstimo
/// </summary>
public class EmprestimoItemRequest
{
    /// <summary>
    /// ID do livro a ser emprestado
    /// </summary>
    /// <example>5</example>
    public int LivroId { get; set; }

    /// <summary>
    /// Quantidade de dias do empréstimo para este livro (máximo 90 dias)
    /// </summary>
    /// <example>14</example>
    public int DiasEmprestimo { get; set; }
}
