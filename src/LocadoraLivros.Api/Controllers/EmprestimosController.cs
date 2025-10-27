using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

/// <summary>
/// Gerenciamento de empr�stimos de livros
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _emprestimoService;

    public EmprestimosController(IEmprestimoService emprestimoService)
    {
        _emprestimoService = emprestimoService;
    }

    /// <summary>
    /// Realiza um novo empr�stimo de um ou mais livros
    /// </summary>
    [HttpPost("realizar")]
    [ProducesResponseType(typeof(ApiResponse<EmprestimoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EmprestimoResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EmprestimoResponseDto>>> RealizarEmprestimo(
        [FromBody] RealizarEmprestimoRequest request)
    {
        var (success, message, data) = await _emprestimoService.RealizarEmprestimoAsync(request);

        if (!success)
            return BadRequest(new ApiResponse<EmprestimoResponseDto>(message));

        return Ok(new ApiResponse<EmprestimoResponseDto>(data, message));
    }

    /// <summary>
    /// Lista todos os empr�stimos com pagina��o
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmprestimoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmprestimoResponseDto>>>> GetAll(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _emprestimoService.GetAllAsync(parameters);
        return Ok(new ApiResponse<PagedResult<EmprestimoResponseDto>>(result));
    }

    /// <summary>
    /// Busca um empr�stimo por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EmprestimoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EmprestimoResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EmprestimoResponseDto>>> GetById(int id)
    {
        var emprestimo = await _emprestimoService.GetByIdAsync(id);

        if (emprestimo == null)
            return NotFound(new ApiResponse<EmprestimoResponseDto>("Empr�stimo n�o encontrado"));

        return Ok(new ApiResponse<EmprestimoResponseDto>(emprestimo));
    }

    /// <summary>
    /// Lista empr�stimos ativos (n�o devolvidos)
    /// </summary>
    [HttpGet("ativos")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmprestimoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmprestimoResponseDto>>>> GetAtivos(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _emprestimoService.GetAtivosAsync(parameters);
        return Ok(new ApiResponse<PagedResult<EmprestimoResponseDto>>(result));
    }

    /// <summary>
    /// Lista empr�stimos atrasados
    /// </summary>
    [HttpGet("atrasados")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmprestimoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmprestimoResponseDto>>>> GetAtrasados(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _emprestimoService.GetAtrasadosAsync(parameters);
        return Ok(new ApiResponse<PagedResult<EmprestimoResponseDto>>(result));
    }

    /// <summary>
    /// Lista empr�stimos de um cliente espec�fico
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmprestimoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmprestimoResponseDto>>>> GetByCliente(
        int clienteId,
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _emprestimoService.GetByClienteIdAsync(clienteId, parameters);
        return Ok(new ApiResponse<PagedResult<EmprestimoResponseDto>>(result));
    }

    /// <summary>
    /// Calcula o valor total de um empr�stimo (incluindo multas se houver)
    /// </summary>
    [HttpGet("{id}/valor-total")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<decimal>>> GetValorTotal(int id)
    {
        var valor = await _emprestimoService.CalcularValorTotalAsync(id);
        return Ok(new ApiResponse<decimal>(valor));
    }

    /// <summary>
    /// Devolve um empr�stimo completo (todos os itens)
    /// </summary>
    [HttpPost("{id}/devolver")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<bool>>> DevolverEmprestimo(int id)
    {
        var (success, message) = await _emprestimoService.DevolverEmprestimoAsync(id);

        if (!success)
            return BadRequest(new ApiResponse<bool>(message));

        return Ok(new ApiResponse<bool>(true, message));
    }

    /// <summary>
    /// Devolve um item espec�fico do empr�stimo
    /// </summary>
    [HttpPost("item/{itemId}/devolver")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<bool>>> DevolverItem(int itemId)
    {
        var (success, message) = await _emprestimoService.DevolverItemAsync(itemId);

        if (!success)
            return BadRequest(new ApiResponse<bool>(message));

        return Ok(new ApiResponse<bool>(true, message));
    }
}
