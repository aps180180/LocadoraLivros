using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.User;
using LocadoraLivros.Api.Shared.Constants;

namespace LocadoraLivros.Api.Validators;

public class RemoveRoleRequestValidator : AbstractValidator<RemoveRoleRequest>
{
    public RemoveRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("O nome da role é obrigatório")
            .Must(role => role == Roles.Admin || role == Roles.Manager || role == Roles.User)
            .WithMessage($"Role inválida. Use: {Roles.Admin}, {Roles.Manager} ou {Roles.User}");
    }
}
