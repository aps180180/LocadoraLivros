using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Models.DTOs.Cliente;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Celular { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
        
    public TipoCliente TipoCliente { get; set; }
    public string TipoClienteDescricao { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
