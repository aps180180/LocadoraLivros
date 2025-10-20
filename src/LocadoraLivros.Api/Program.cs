// Program.cs
using LocadoraLivros.Api.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using LocadoraLivros.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Database
builder.Services.ConfigureDatabase(builder.Configuration);

// Identity
builder.Services.ConfigureIdentity();

// JWT
builder.Services.ConfigureJwt(builder.Configuration);

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

// Criar roles na inicialização
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.CreateRoles();
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
app.MapControllers();

app.Run();
