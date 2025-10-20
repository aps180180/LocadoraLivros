// Validators/RefreshTokenDtoValidator.cs
using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Auth;

namespace LocadoraLivros.Api.Validators;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O refresh token é obrigatório");
    }
}
