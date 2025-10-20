// Controllers/AuthController.cs
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

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto model)
    {
        var (success, message, data) = await _authService.RegisterAsync(model);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message, data) = await _authService.LoginAsync(model, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message, data) = await _authService.RefreshTokenAsync(model.RefreshToken, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<AuthResponseDto>(message));

        return Ok(new ApiResponse<AuthResponseDto>(data, message));
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RevokeTokenDto model)
    {
        var ipAddress = GetIpAddress();
        var (success, message) = await _authService.RevokeTokenAsync(model.RefreshToken, ipAddress);

        if (!success)
            return BadRequest(new ApiResponse<bool>(message));

        return Ok(new ApiResponse<bool>(true, message));
    }

    [Authorize]
    [HttpGet("me")]
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
        

    // Endpoint apenas para Admin
    [Authorize(Policy = Policies.AdminOnly)]
    [HttpGet("users")]
    public ActionResult<ApiResponse<object>> GetAllUsers()
    {
        // Lógica para listar usuários
        return Ok(new ApiResponse<object>(new { Message = "Lista de usuários" }));
    }
}
