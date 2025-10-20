// Validators/LoginDtoValidator.cs
using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Auth;

namespace LocadoraLivros.Api.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email � obrigat�rio")
            .EmailAddress().WithMessage("Email inv�lido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha � obrigat�ria");
    }
}
