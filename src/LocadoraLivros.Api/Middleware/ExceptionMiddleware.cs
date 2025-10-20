using LocadoraLivros.Api.Exceptions;
using LocadoraLivros.Api.Models;
using System.Net;
using System.Text.Json;

namespace LocadoraLivros.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro interno no servidor";
        List<string>? errors = null;

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case BusinessException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Erro de validação";
                errors = validationException.Errors;
                break;

            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = "Você não tem permissão para acessar este recurso";
                break;

            default:
                // Em desenvolvimento, mostrar detalhes do erro
                if (_env.IsDevelopment())
                {
                    message = exception.Message;
                    errors = new List<string> { exception.StackTrace ?? string.Empty };
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = errors != null && errors.Any()
            ? new ApiResponse<object>(errors)
            : new ApiResponse<object>(message);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
