namespace LocadoraLivros.Api.Models.DTOs.Emprestimo;

public class RealizarEmprestimoRequest
{
    public int ClienteId { get; set; }
    public List<EmprestimoItemRequest> Itens { get; set; } = new();
    public string? Observacoes { get; set; }
}

public class EmprestimoItemRequest
{
    public int LivroId { get; set; }
    public int DiasEmprestimo { get; set; }
}
