using LocadoraLivros.Api.Models;
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
    /// Retorna todos os livros ativos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Livro>>>> GetAll()
    {
        var livros = await _livroService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<Livro>>(livros));
    }

    /// <summary>
    /// Retorna um livro pelo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Livro>>> GetById(int id)
    {
        var livro = await _livroService.GetByIdAsync(id);

        if (livro == null)
            return NotFound(new ApiResponse<Livro>("Livro não encontrado"));

        return Ok(new ApiResponse<Livro>(livro));
    }

    /// <summary>
    /// Retorna apenas livros disponíveis para empréstimo
    /// </summary>
    [HttpGet("disponiveis")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Livro>>>> GetDisponiveis()
    {
        var livros = await _livroService.GetDisponiveisAsync();
        return Ok(new ApiResponse<IEnumerable<Livro>>(livros));
    }

    /// <summary>
    /// Pesquisa livros por termo (título, autor, ISBN, categoria, editora)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Livro>>>> Search([FromQuery] string termo)
    {
        var livros = await _livroService.SearchAsync(termo);
        return Ok(new ApiResponse<IEnumerable<Livro>>(livros));
    }

    /// <summary>
    /// Retorna livros de uma categoria específica
    /// </summary>
    [HttpGet("categoria/{categoria}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Livro>>>> GetByCategoria(string categoria)
    {
        var livros = await _livroService.GetByCategoriaAsync(categoria);
        return Ok(new ApiResponse<IEnumerable<Livro>>(livros));
    }

    /// <summary>
    /// Cria um novo livro (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Livro>>> Create([FromBody] Livro livro)
    {
        var created = await _livroService.CreateAsync(livro);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new ApiResponse<Livro>(created, "Livro criado com sucesso"));
    }

    /// <summary>
    /// Atualiza um livro existente (requer permissão Admin ou Manager)
    /// </summary>
    [Authorize(Policy = Policies.AdminOrManager)]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Livro>>> Update(int id, [FromBody] Livro livro)
    {
        if (id != livro.Id)
            return BadRequest(new ApiResponse<Livro>("ID incompatível"));

        var updated = await _livroService.UpdateAsync(livro);
        return Ok(new ApiResponse<Livro>(updated, "Livro atualizado com sucesso"));
    }

    /// <summary>
    /// Exclui um livro (requer permissão Admin)
    /// </summary>
    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id}")]
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
    public async Task<ActionResult<ApiResponse<string>>> UploadImagem(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse<string>("Nenhum arquivo foi enviado"));

        // Validar tamanho (máximo 10MB)
        if (file.Length > 10485760)
            return BadRequest(new ApiResponse<string>("O arquivo excede o tamanho máximo de 10MB"));

        // Validar extensão
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
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImagem([FromQuery] string url)
    {
        await _storageService.DeleteFileAsync(url);
        return Ok(new ApiResponse<bool>(true, "Imagem excluída com sucesso"));
    }
}
