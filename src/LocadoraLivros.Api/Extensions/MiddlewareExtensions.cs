using LocadoraLivros.Api.Middleware;

namespace LocadoraLivros.Api.Extensions;

public static class MiddlewareExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
