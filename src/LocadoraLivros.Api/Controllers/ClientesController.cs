using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Cliente;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

/// <summary>
/// Gerenciamento de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<ClienteDto>>>> GetAll(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _clienteService.GetAllAsync(parameters);
        return Ok(new ApiResponse<PagedResult<ClienteDto>>(result));
    }

    /// <summary>
    /// Busca um cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);

        if (cliente == null)
            return NotFound(new ApiResponse<ClienteDto>("Cliente não encontrado"));

        return Ok(new ApiResponse<ClienteDto>(cliente));
    }

    /// <summary>
    /// Busca um cliente por CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> GetByCpf(string cpf)
    {
        var cliente = await _clienteService.GetByCpfAsync(cpf);

        if (cliente == null)
            return NotFound(new ApiResponse<ClienteDto>("Cliente não encontrado"));

        return Ok(new ApiResponse<ClienteDto>(cliente));
    }

    /// <summary>
    /// Busca clientes por termo (nome, CPF, email, cidade)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<ClienteDto>>>> Search(
        [FromQuery] string termo,
        [FromQuery] PaginationParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return BadRequest(new ApiResponse<PagedResult<ClienteDto>>("Termo de busca é obrigatório"));

        var result = await _clienteService.SearchAsync(termo, parameters);
        return Ok(new ApiResponse<PagedResult<ClienteDto>>(result));
    }

    /// <summary>
    /// Cadastra um novo cliente
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> Create([FromBody] CreateClienteDto dto)
    {
        var (success, message, data) = await _clienteService.CreateAsync(dto);

        if (!success)
            return BadRequest(new ApiResponse<ClienteDto>(message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = data!.Id },
            new ApiResponse<ClienteDto>(data, message));
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        var (success, message, data) = await _clienteService.UpdateAsync(id, dto);

        if (!success)
        {
            if (message == "Cliente não encontrado")
                return NotFound(new ApiResponse<ClienteDto>(message));

            return BadRequest(new ApiResponse<ClienteDto>(message));
        }

        return Ok(new ApiResponse<ClienteDto>(data, message));
    }

    /// <summary>
    /// Exclui (desativa) um cliente
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var (success, message) = await _clienteService.DeleteAsync(id);

        if (!success)
        {
            if (message == "Cliente não encontrado")
                return NotFound(new ApiResponse<bool>(message));

            return BadRequest(new ApiResponse<bool>(message));
        }

        return Ok(new ApiResponse<bool>(true, message));
    }
}
