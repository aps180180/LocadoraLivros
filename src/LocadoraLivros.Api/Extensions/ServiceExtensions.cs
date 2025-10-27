// Extensions/ServiceExtensions.cs
using LocadoraLivros.Api.Data;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Models.Settings;
using LocadoraLivros.Api.Services;
using LocadoraLivros.Api.Services.Interfaces;
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LocadoraLivros.Api.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configura��es de senha
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;

            // Configura��es de usu�rio
            options.User.RequireUniqueEmail = true;

            // Configura��es de lockout
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettings);

        var secretKey = jwtSettings["SecretKey"];
        var key = Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT SecretKey not configured"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // true em produ��o
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Pol�tica: Apenas Admin
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.RequireRole(Roles.Admin));

            // Pol�tica: Admin ou Manager
            options.AddPolicy(Policies.AdminOrManager, policy =>
                policy.RequireRole(Roles.Admin, Roles.Manager));

            // Pol�tica: Apenas Manager
            options.AddPolicy(Policies.ManagerOnly, policy =>
                policy.RequireRole(Roles.Manager));

            // Pol�tica: Usu�rio ativo (autenticado)
            options.AddPolicy(Policies.ActiveUser, policy =>
                policy.RequireAssertion(context =>
                    context.User.Identity?.IsAuthenticated == true));

            // Pol�tica: Email confirmado
            options.AddPolicy(Policies.EmailConfirmed, policy =>
                policy.RequireAssertion(context =>
                {
                    var emailConfirmed = context.User.FindFirst("EmailConfirmed")?.Value;
                    return emailConfirmed == "True";
                }));

            // Pol�tica: Requer claim espec�fico
            options.AddPolicy("RequireFullName", policy =>
                policy.RequireClaim("NomeCompleto"));
        });
    }




    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Locadora de Livros API",
                Version = "v1",
                Description = "API para gerenciamento de locadora de livros com autentica��o JWT",
                Contact = new OpenApiContact
                {
                    Name = "Seu Nome",
                    Email = "seu@email.com"
                }
            });

            // Configurar JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,  
                Scheme = "Bearer",               
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header usando Bearer scheme. \r\n\r\n" +
                             "Digite seu token no campo abaixo.\r\n\r\n" +
                             "Exemplo: '12345abcdef'"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });

            // Incluir coment�rios XML (se existir)
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
    }


    public static void RegisterServices(this IServiceCollection services)
    {
        // Servi�os de autentica��o
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        /// Servi�os de neg�cio
        services.AddScoped<ILivroService, LivroService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEmprestimoService, EmprestimoService>();
        services.AddScoped<IStorageService, StorageService>();
    }

    public static async Task CreateRoles(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { Roles.Admin, Roles.Manager, Roles.User };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Criar usu�rio admin padr�o (opcional)
        var adminEmail = "admin@locadora.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                NomeCompleto = "Administrador",
                DataCadastro = DateTime.UtcNow,
                Ativo = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
