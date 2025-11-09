using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.DTOs.Livro;
using LocadoraLivros.Api.Models.Pagination;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LocadoraLivros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _livroService;
    private readonly IStorageService _storageService;

    public LivrosController(ILivroService livroService, IStorageService storageService)
    {
        _livroService = livroService;
        _storageService = storageService;
    }

    /// <summary>
    /// Retorna todos os livros ativos com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LivroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<LivroDto>>>> GetAll([FromQuery] PaginationParameters parameters)
    {
        var result = await _livroService.GetAllAsync(parameters);
        return Ok(new ApiResponse<PagedResult<LivroDto>>(result));
    }

    /// <summary>
    /// Retorna um livro pelo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LivroDto>>> GetById(int id)
    {
        var livro = await _livroService.GetByIdAsync(id);

        if (livro == null)
            return NotFound(new ApiResponse<LivroDto>("Livro não encontrado"));

        return Ok(new ApiResponse<LivroDto>(livro));
    }

    /// <summary>
    /// Retorna apenas livros disponíveis para empréstimo com paginação
    /// </summary>
    [HttpGet("disponiveis")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LivroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<LivroDto>>>> GetDisponiveis([FromQuery] PaginationParameters parameters)
    {
        var result = await _livroService.GetDisponiveisAsync(parameters);
        return Ok(new ApiResponse<PagedResult<LivroDto>>(result));
    }

    /// <summary>
    /// Pesquisa livros por termo com paginação (título, autor, ISBN, categoria, editora)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LivroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<LivroDto>>>> Search(
        [FromQuery] string termo,
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _livroService.SearchAsync(termo, parameters);
        return Ok(new ApiResponse<PagedResult<LivroDto>>(result));
    }

    /// <summary>
    /// Retorna livros de uma categoria específica com paginação
    /// </summary>
    [HttpGet("categoria/{categoria}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LivroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<LivroDto>>>> GetByCategoria(
        string categoria,
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _livroService.GetByCategoriaAsync(categoria, parameters);
        return Ok(new ApiResponse<PagedResult<LivroDto>>(result));
    }
    /// <summary>
    /// Cria um novo livro (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<LivroDto>>> Create([FromBody] CreateLivroDto livro)
    {
        var (success, message, data) = await _livroService.CreateAsync(livro);

        if (!success)
            return BadRequest(new ApiResponse<LivroDto>(message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = data!.Id },
            new ApiResponse<LivroDto>(data, message));
    }
        

    /// <summary>
    /// Atualiza um livro existente (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LivroDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<LivroDto>>> Update(int id, [FromBody] UpdateLivroDto livro)
    {
        var (success, message, data) = await _livroService.UpdateAsync(id, livro);

        if (!success)
        {
            if (message == "Livro não encontrado")
                return NotFound(new ApiResponse<LivroDto>(message));

            return BadRequest(new ApiResponse<LivroDto>(message));
        }

        return Ok(new ApiResponse<LivroDto>(data, message));
    }

    /// <summary>
    /// Exclui um livro (requer permissão Admin)
    /// </summary>
    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        await _livroService.DeleteAsync(id);
        return Ok(new ApiResponse<bool>(true, "Livro excluído com sucesso"));
    }

    /// <summary>
    /// Upload de imagem do livro (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpPost("upload-imagem")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<string>>> UploadImagem(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse<string>("Nenhum arquivo foi enviado"));

        if (file.Length > 10485760)
            return BadRequest(new ApiResponse<string>("O arquivo excede o tamanho máximo de 10MB"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new ApiResponse<string>("Formato de arquivo não permitido. Use: JPG, PNG, GIF ou WebP"));

        using var stream = file.OpenReadStream();
        var url = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

        return Ok(new ApiResponse<string>(url, "Imagem enviada com sucesso"));
    }

    /// <summary>
    /// Deleta uma imagem do servidor (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpDelete("imagem")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImagem([FromQuery] string url)
    {
        await _storageService.DeleteFileAsync(url);
        return Ok(new ApiResponse<bool>(true, "Imagem excluída com sucesso"));
    }
}
