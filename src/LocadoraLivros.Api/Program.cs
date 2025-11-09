// Program.cs
using FluentValidation;
using FluentValidation.AspNetCore;
using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Extensions;
using LocadoraLivros.Api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Database
builder.Services.ConfigureDatabase(builder.Configuration);

// Identity
builder.Services.ConfigureIdentity();

// JWT Authentication
builder.Services.ConfigureJwt(builder.Configuration);
//Autorization
builder.Services.ConfigureAuthorization();
// CORS
builder.Services.ConfigureCors();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Registrar serviços customizados
builder.Services.RegisterServices();

// Configurar comportamento de validação do ModelState
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new ApiResponse<object>(errors));
    };
});

var app = builder.Build();

// Criar roles e popular dados iniciais
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Criar roles (Admin, Manager, User)
        await services.CreateRoles();

        // Popular dados iniciais (Seed)
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar banco de dados");
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ADICIONAR ANTES DE UseHttpsRedirection
app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles(); // Para servir imagens do /wwwroot/uploads
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// ✅ ADICIONAR ISSO AQUI (TEMPORÁRIO PARA DEBUG)
app.Use(async (context, next) =>
{
    Console.WriteLine($"=== REQUEST: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
    Console.WriteLine($"User.Identity.IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"User.Identity.Name: {context.User?.Identity?.Name}");
    await next();
});

app.MapControllers();

app.Run();
