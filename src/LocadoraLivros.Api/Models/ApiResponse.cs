// Models/ApiResponse.cs
namespace LocadoraLivros.Api.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public ApiResponse(T? data, string? message = null)
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public ApiResponse(string? message)
    {
        Success = false;
        Message = message;
    }

    public ApiResponse(List<string> errors)
    {
        Success = false;
        Errors = errors;
    }
}
