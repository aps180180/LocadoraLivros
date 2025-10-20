using LocadoraLivros.Api.Services.Interfaces;

namespace LocadoraLivros.Api.Services;

public class StorageService : IStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadPath;
    private readonly ILogger<StorageService> _logger;

    public StorageService(IWebHostEnvironment environment, ILogger<StorageService> logger)
    {
        _environment = environment;
        _logger = logger;
        _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

        // Criar pasta se não existir
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        // Gerar nome único
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Salvar arquivo
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        _logger.LogInformation("Arquivo salvo: {FileName}", uniqueFileName);

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.CompletedTask;

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_uploadPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("Arquivo deletado: {FileName}", fileName);
        }

        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.FromResult(false);

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_uploadPath, fileName);

        return Task.FromResult(File.Exists(filePath));
    }
}
