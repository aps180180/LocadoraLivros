using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

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
    /// Retorna todos os empr�stimos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Emprestimo>>>> GetAll()
    {
        var emprestimos = await _emprestimoService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<Emprestimo>>(emprestimos));
    }

    /// <summary>
    /// Retorna um empr�stimo pelo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Emprestimo>>> GetById(int id)
    {
        var emprestimo = await _emprestimoService.GetByIdAsync(id);

        if (emprestimo == null)
            return NotFound(new ApiResponse<Emprestimo>("Empr�stimo n�o encontrado"));

        return Ok(new ApiResponse<Emprestimo>(emprestimo));
    }

    /// <summary>
    /// Retorna apenas empr�stimos ativos
    /// </summary>
    [HttpGet("ativos")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Emprestimo>>>> GetAtivos()
    {
        var emprestimos = await _emprestimoService.GetAtivosAsync();
        return Ok(new ApiResponse<IEnumerable<Emprestimo>>(emprestimos));
    }

    /// <summary>
    /// Retorna empr�stimos atrasados
    /// </summary>
    [HttpGet("atrasados")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Emprestimo>>>> GetAtrasados()
    {
        var emprestimos = await _emprestimoService.GetAtrasadosAsync();
        return Ok(new ApiResponse<IEnumerable<Emprestimo>>(emprestimos));
    }

    /// <summary>
    /// Retorna empr�stimos de um cliente espec�fico
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Emprestimo>>>> GetByCliente(int clienteId)
    {
        var emprestimos = await _emprestimoService.GetByClienteIdAsync(clienteId);
        return Ok(new ApiResponse<IEnumerable<Emprestimo>>(emprestimos));
    }

    /// <summary>
    /// Realiza um novo empr�stimo
    /// </summary>
    [HttpPost("realizar")]
    public async Task<ActionResult<ApiResponse<Emprestimo>>> RealizarEmprestimo(
        [FromBody] RealizarEmprestimoRequest request)
    {
        // Converter DTO para formato que o service espera
        var itens = request.Itens.Select(i => (i.LivroId, i.DiasEmprestimo)).ToList();

        var emprestimo = await _emprestimoService.RealizarEmprestimoAsync(
            request.ClienteId,
            itens,
            request.Observacoes);

        return Ok(new ApiResponse<Emprestimo>(emprestimo, "Empr�stimo realizado com sucesso"));
    }

    /// <summary>
    /// Realiza a devolu��o completa de um empr�stimo
    /// </summary>
    [HttpPost("{id}/devolver")]
    public async Task<ActionResult<ApiResponse<Emprestimo>>> RealizarDevolucao(int id)
    {
        var emprestimo = await _emprestimoService.RealizarDevolucaoAsync(id);
        return Ok(new ApiResponse<Emprestimo>(emprestimo, "Devolu��o realizada com sucesso"));
    }

    /// <summary>
    /// Devolve um item espec�fico do empr�stimo (devolu��o parcial)
    /// </summary>
    [HttpPost("item/{itemId}/devolver")]
    public async Task<ActionResult<ApiResponse<Emprestimo>>> DevolverItem(int itemId)
    {
        var emprestimo = await _emprestimoService.DevolverItemAsync(itemId);
        return Ok(new ApiResponse<Emprestimo>(emprestimo, "Item devolvido com sucesso"));
    }

    /// <summary>
    /// Calcula o valor total de um empr�stimo
    /// </summary>
    [HttpGet("{id}/valor-total")]
    public async Task<ActionResult<ApiResponse<decimal>>> CalcularValorTotal(int id)
    {
        var valorTotal = await _emprestimoService.CalcularValorTotalAsync(id);
        return Ok(new ApiResponse<decimal>(valorTotal));
    }
}

/// <summary>
/// Request para realizar empr�stimo
/// </summary>
