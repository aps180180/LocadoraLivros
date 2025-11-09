using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraLivros.Api.Controllers;

/// <summary>
/// Gerenciamento de configurações do sistema de empréstimos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.AdminOnly)]
public class ConfiguracaoController : ControllerBase
{
    private readonly IConfiguracaoService _configuracaoService;

    public ConfiguracaoController(IConfiguracaoService configuracaoService)
    {
        _configuracaoService = configuracaoService;
    }

    /// <summary>
    /// Obtém a configuração atual do sistema
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ConfiguracaoEmprestimo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ConfiguracaoEmprestimo>>> GetConfiguracao()
    {
        var config = await _configuracaoService.GetConfiguracaoAtualAsync();
        return Ok(new ApiResponse<ConfiguracaoEmprestimo>(config));
    }

    /// <summary>
    /// Atualiza a configuração do sistema
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ConfiguracaoEmprestimo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConfiguracaoEmprestimo>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ConfiguracaoEmprestimo>>> AtualizarConfiguracao(
        [FromBody] ConfiguracaoEmprestimo config)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var resultado = await _configuracaoService.AtualizarConfiguracaoAsync(config, userId);

        return Ok(new ApiResponse<ConfiguracaoEmprestimo>(
            resultado,
            "Configuração atualizada com sucesso"));
    }

    /// <summary>
    /// Limpa o cache de configuração (força reload do banco)
    /// </summary>
    [HttpPost("limpar-cache")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<ApiResponse<bool>> LimparCache()
    {
        _configuracaoService.LimparCache();
        return Ok(new ApiResponse<bool>(true, "Cache limpo com sucesso"));
    }
}
