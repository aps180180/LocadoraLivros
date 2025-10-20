// Shared/Authorization/AuthorizationPolicies.cs
using LocadoraLivros.Api.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace LocadoraLivros.Api.Shared.Authorization;

public static class AuthorizationPolicies
{
    public static void ConfigurePolicies(AuthorizationOptions options)
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
    }
}
