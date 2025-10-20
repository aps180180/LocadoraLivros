// Validators/RegisterDtoValidator.cs
using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Auth;

namespace LocadoraLivros.Api.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("O nome completo é obrigatório")
            .MinimumLength(3).WithMessage("O nome completo deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("O nome completo deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(150).WithMessage("O email deve ter no máximo 150 caracteres");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("O nome de usuário é obrigatório")
            .MinimumLength(3).WithMessage("O nome de usuário deve ter no mínimo 3 caracteres")
            .MaximumLength(50).WithMessage("O nome de usuário deve ter no máximo 50 caracteres")
            .Matches("^[a-zA-Z0-9_]*$").WithMessage("O nome de usuário pode conter apenas letras, números e underscore");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("A confirmação de senha é obrigatória")
            .Equal(x => x.Password).WithMessage("As senhas não conferem");
    }
}
