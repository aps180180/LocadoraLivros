using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Auth;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto model)
    {
        var (success, message, data) = await _authService.RegisterAsync(model);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    /// <summary>
    /// Realiza login e retorna token JWT
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message, data) = await _authService.LoginAsync(model, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    /// <summary>
    /// Renova o token JWT usando refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message, data) = await _authService.RefreshTokenAsync(model.RefreshToken, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    /// <summary>
    /// Revoga um refresh token
    /// </summary>
    [Authorize]
    [HttpPost("revoke-token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RevokeTokenDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message) = await _authService.RevokeTokenAsync(model.RefreshToken, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<bool>(message));

        return Ok(new ApiResponse<bool>(true, message));
    }

    /// <summary>
    /// Retorna informações do usuário autenticado
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
        var user = new
        {
            Id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            UserName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            NomeCompleto = User.FindFirst("NomeCompleto")?.Value,
            Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList()
        };

        return Ok(new ApiResponse<object>(user));
    }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"].ToString();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
    /// <summary>
    /// Retorna informações detalhadas do usuário autenticado incluindo roles e policies
    /// </summary>
    [Authorize]
    [HttpGet("me/permissions")]
    [ProducesResponseType(typeof(ApiResponse<UserPermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<UserPermissionsDto>> GetCurrentUserPermissions()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var nomeCompleto = User.FindFirst("NomeCompleto")?.Value;

        // Obter roles do JWT
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Calcular policies baseado nas roles
        var hasAdminRole = roles.Contains(Roles.Admin);
        var hasManagerRole = roles.Contains(Roles.Manager);
        var hasUserRole = roles.Contains(Roles.User);

        var policies = new List<string>();

        if (hasAdminRole)
        {
            policies.Add(Policies.AdminOnly);
            policies.Add(Policies.AdminOrManager);
        }
        else if (hasManagerRole)
        {
            policies.Add(Policies.AdminOrManager);
        }

        var permissions = new UserPermissionsDto
        {
            Id = userId ?? "",
            UserName = userName ?? "",
            Email = email ?? "",
            NomeCompleto = nomeCompleto ?? "",
            Roles = roles,
            Policies = policies,
            CanCreateBooks = hasAdminRole || hasManagerRole,
            CanEditBooks = hasAdminRole || hasManagerRole,
            CanDeleteBooks = hasAdminRole,
            CanDeleteClients = hasAdminRole,
            CanManageUsers = hasAdminRole,
            CanManageRoles = hasAdminRole
        };

        return Ok(new ApiResponse<UserPermissionsDto>(permissions));
    }

}
