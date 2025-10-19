// Services/Interfaces/IAuthService.cs
using LocadoraLivros.Api.Models.DTOs.Auth;

namespace LocadoraLivros.Api.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string? Message, AuthResponseDto? Data)> RegisterAsync(RegisterDto model);
    Task<(bool Success, string? Message, AuthResponseDto? Data)> LoginAsync(LoginDto model, string ipAddress);
    Task<(bool Success, string? Message, AuthResponseDto? Data)> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<(bool Success, string? Message)> RevokeTokenAsync(string refreshToken, string ipAddress);
}
