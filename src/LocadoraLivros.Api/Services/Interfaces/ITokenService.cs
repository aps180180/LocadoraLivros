// Services/Interfaces/ITokenService.cs
using LocadoraLivros.Api.Models;
using System.Security.Claims;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    RefreshToken GenerateRefreshToken(string ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<bool> SaveRefreshTokenAsync(string userId, RefreshToken refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress, string reason);
    Task RevokeAllUserRefreshTokensAsync(string userId, string ipAddress, string reason);
    Task RemoveOldRefreshTokensAsync(string userId);
}
