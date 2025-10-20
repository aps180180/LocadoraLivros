// Services/AuthService.cs
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Auth;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace LocadoraLivros.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<(bool Success, string? Message, AuthResponseDto? Data)> RegisterAsync(RegisterDto model)
    {
        // Verificar se o email já existe
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return (false, "Email já cadastrado", null);

        // Verificar se o username já existe
        var existingUsername = await _userManager.FindByNameAsync(model.UserName);
        if (existingUsername != null)
            return (false, "Nome de usuário já cadastrado", null);

        // Criar novo usuário
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            NomeCompleto = model.NomeCompleto,
            DataCadastro = DateTime.UtcNow,
            Ativo = true,
            EmailConfirmed = true // Para simplificar, confirmamos automaticamente
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errors, null);
        }

        // Adicionar role padrão
        await _userManager.AddToRoleAsync(user, Roles.User);

        return (true, "Usuário registrado com sucesso", null);
    }

    public async Task<(bool Success, string? Message, AuthResponseDto? Data)> LoginAsync(LoginDto model, string ipAddress)
    {
        // Buscar usuário por email
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return (false, "Email ou senha incorretos", null);

        // Verificar se o usuário está ativo
        if (!user.Ativo)
            return (false, "Usuário inativo", null);

        // Verificar senha
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return (false, "Email ou senha incorretos", null);

        // Obter roles do usuário
        var roles = await _userManager.GetRolesAsync(user);

        // Gerar tokens
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken(ipAddress);

        // Salvar refresh token
        await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

        // Remover tokens antigos
        await _tokenService.RemoveOldRefreshTokensAsync(user.Id);

        // Criar resposta
        var authResponse = new AuthResponseDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles.ToList(),
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(60) // Deve vir do JwtSettings
        };

        return (true, "Login realizado com sucesso", authResponse);
    }

    public async Task<(bool Success, string? Message, AuthResponseDto? Data)> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        // Buscar refresh token
        var token = await _tokenService.GetRefreshTokenAsync(refreshToken);

        if (token == null)
            return (false, "Token inválido", null);

        if (!token.IsActive)
            return (false, "Token inválido ou expirado", null);

        var user = token.User;

        if (!user.Ativo)
            return (false, "Usuário inativo", null);

        // Gerar novo refresh token
        var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress);
        newRefreshToken.ReplacedByToken = token.Token;

        // Revogar o token antigo
        await _tokenService.RevokeRefreshTokenAsync(token.Token, ipAddress, "Substituído por novo token");

        // Salvar novo token
        await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

        // Gerar novo access token
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        // Remover tokens antigos
        await _tokenService.RemoveOldRefreshTokensAsync(user.Id);

        var authResponse = new AuthResponseDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles.ToList(),
            Token = accessToken,
            RefreshToken = newRefreshToken.Token,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        return (true, "Token atualizado com sucesso", authResponse);
    }

    public async Task<(bool Success, string? Message)> RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        var token = await _tokenService.GetRefreshTokenAsync(refreshToken);

        if (token == null || !token.IsActive)
            return (false, "Token inválido");

        var success = await _tokenService.RevokeRefreshTokenAsync(refreshToken, ipAddress, "Revogado pelo usuário");

        return success
            ? (true, "Token revogado com sucesso")
            : (false, "Erro ao revogar token");
    }
}
