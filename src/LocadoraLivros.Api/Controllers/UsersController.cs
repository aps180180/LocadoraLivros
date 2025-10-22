using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.User;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Controllers;

/// <summary>
/// Gerenciamento de usu�rios e roles (apenas Admin)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.AdminOnly)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os usu�rios do sistema
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                Ativo = user.Ativo,
                DataCadastro = user.DataCadastro
            });
        }

        return Ok(new ApiResponse<IEnumerable<UserDto>>(userDtos));
    }

    /// <summary>
    /// Busca um usu�rio espec�fico pelo ID
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound(new ApiResponse<UserDto>("Usu�rio n�o encontrado"));

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            Ativo = user.Ativo,
            DataCadastro = user.DataCadastro
        };

        return Ok(new ApiResponse<UserDto>(userDto));
    }

    /// <summary>
    /// Lista todas as roles dispon�veis no sistema
    /// </summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<ApiResponse<IEnumerable<string>>> GetRoles()
    {
        var roles = new[] { Roles.Admin, Roles.Manager, Roles.User };
        return Ok(new ApiResponse<IEnumerable<string>>(roles));
    }

    /// <summary>
    /// Adiciona uma role a um usu�rio
    /// </summary>
    [HttpPost("{userId}/roles")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> AddRole(
        string userId,
        [FromBody] AddRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound(new ApiResponse<UserDto>("Usu�rio n�o encontrado"));

        // Verificar se j� tem a role
        var hasRole = await _userManager.IsInRoleAsync(user, request.RoleName);
        if (hasRole)
            return BadRequest(new ApiResponse<UserDto>($"Usu�rio j� possui a role {request.RoleName}"));

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ApiResponse<UserDto>(errors));
        }

        _logger.LogInformation("Role {Role} adicionada ao usu�rio {UserId} por {AdminId}",
            request.RoleName, userId, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        // Retornar usu�rio atualizado
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            Ativo = user.Ativo,
            DataCadastro = user.DataCadastro
        };

        return Ok(new ApiResponse<UserDto>(userDto, $"Role {request.RoleName} adicionada com sucesso"));
    }

    /// <summary>
    /// Remove uma role de um usu�rio
    /// </summary>
    [HttpDelete("{userId}/roles")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> RemoveRole(
        string userId,
        [FromBody] RemoveRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound(new ApiResponse<UserDto>("Usu�rio n�o encontrado"));

        // Verificar se tem a role
        var hasRole = await _userManager.IsInRoleAsync(user, request.RoleName);
        if (!hasRole)
            return BadRequest(new ApiResponse<UserDto>($"Usu�rio n�o possui a role {request.RoleName}"));

        // Proteger: n�o pode remover a �ltima role Admin do sistema
        if (request.RoleName == Roles.Admin)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count == 1)
                return BadRequest(new ApiResponse<UserDto>("N�o � poss�vel remover a role Admin do �ltimo administrador do sistema"));
        }

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ApiResponse<UserDto>(errors));
        }

        _logger.LogInformation("Role {Role} removida do usu�rio {UserId} por {AdminId}",
            request.RoleName, userId, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        // Retornar usu�rio atualizado
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            Ativo = user.Ativo,
            DataCadastro = user.DataCadastro
        };

        return Ok(new ApiResponse<UserDto>(userDto, $"Role {request.RoleName} removida com sucesso"));
    }

    /// <summary>
    /// Ativa ou desativa um usu�rio
    /// </summary>
    [HttpPatch("{userId}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> ToggleStatus(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound(new ApiResponse<UserDto>("Usu�rio n�o encontrado"));

        // N�o pode desativar o pr�prio usu�rio
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == currentUserId)
            return BadRequest(new ApiResponse<UserDto>("Voc� n�o pode desativar sua pr�pria conta"));

        user.Ativo = !user.Ativo;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Status do usu�rio {UserId} alterado para {Status} por {AdminId}",
            userId, user.Ativo ? "Ativo" : "Inativo", currentUserId);

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            Ativo = user.Ativo,
            DataCadastro = user.DataCadastro
        };

        return Ok(new ApiResponse<UserDto>(userDto, $"Usu�rio {(user.Ativo ? "ativado" : "desativado")} com sucesso"));
    }
}
