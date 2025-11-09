using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Models.DTOs.Cliente;

public class UpdateClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Celular { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
        
    public TipoCliente TipoCliente { get; set; }
}
