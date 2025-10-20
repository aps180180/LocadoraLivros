// Shared/Constants/Policies.cs
namespace LocadoraLivros.Api.Shared.Constants;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string AdminOrManager = "AdminOrManager";
    public const string ManagerOnly = "ManagerOnly";
    public const string ActiveUser = "ActiveUser";
    public const string EmailConfirmed = "EmailConfirmed";
}
