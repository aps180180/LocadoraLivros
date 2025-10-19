using Microsoft.AspNetCore.Identity;

namespace LocadoraLivros.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } 
    public bool Ativo { get; set; }

    // Relacionamento com RefreshTokens
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
