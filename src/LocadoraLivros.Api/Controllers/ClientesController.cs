using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

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
    /// Retorna todos os clientes ativos com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<Cliente>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<Cliente>>>> GetAll([FromQuery] PaginationParameters parameters)
    {
        var result = await _clienteService.GetAllAsync(parameters);
        return Ok(new ApiResponse<PagedResult<Cliente>>(result));
    }

    /// <summary>
    /// Retorna um cliente pelo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cliente>>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);

        if (cliente == null)
            return NotFound(new ApiResponse<Cliente>("Cliente não encontrado"));

        return Ok(new ApiResponse<Cliente>(cliente));
    }

    /// <summary>
    /// Retorna um cliente pelo CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cliente>>> GetByCpf(string cpf)
    {
        var cliente = await _clienteService.GetByCpfAsync(cpf);

        if (cliente == null)
            return NotFound(new ApiResponse<Cliente>("Cliente não encontrado"));

        return Ok(new ApiResponse<Cliente>(cliente));
    }

    /// <summary>
    /// Pesquisa clientes por termo com paginação (nome, CPF, email, telefone, cidade)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<Cliente>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<Cliente>>>> Search(
        [FromQuery] string termo,
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _clienteService.SearchAsync(termo, parameters);
        return Ok(new ApiResponse<PagedResult<Cliente>>(result));
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cliente>>> Create([FromBody] Cliente cliente)
    {
        var created = await _clienteService.CreateAsync(cliente);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new ApiResponse<Cliente>(created, "Cliente criado com sucesso"));
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<Cliente>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cliente>>> Update(int id, [FromBody] Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest(new ApiResponse<Cliente>("ID incompatível"));

        var updated = await _clienteService.UpdateAsync(cliente);
        return Ok(new ApiResponse<Cliente>(updated, "Cliente atualizado com sucesso"));
    }

    /// <summary>
    /// Exclui um cliente (requer permissão Admin)
    /// </summary>
    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        await _clienteService.DeleteAsync(id);
        return Ok(new ApiResponse<bool>(true, "Cliente excluído com sucesso"));
    }
}
