// Validators/RegisterDtoValidator.cs
using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Auth;

namespace LocadoraLivros.Api.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("O nome completo � obrigat�rio")
            .MinimumLength(3).WithMessage("O nome completo deve ter no m�nimo 3 caracteres")
            .MaximumLength(200).WithMessage("O nome completo deve ter no m�ximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email � obrigat�rio")
            .EmailAddress().WithMessage("Email inv�lido")
            .MaximumLength(150).WithMessage("O email deve ter no m�ximo 150 caracteres");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("O nome de usu�rio � obrigat�rio")
            .MinimumLength(3).WithMessage("O nome de usu�rio deve ter no m�nimo 3 caracteres")
            .MaximumLength(50).WithMessage("O nome de usu�rio deve ter no m�ximo 50 caracteres")
            .Matches("^[a-zA-Z0-9_]*$").WithMessage("O nome de usu�rio pode conter apenas letras, n�meros e underscore");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha � obrigat�ria")
            .MinimumLength(6).WithMessage("A senha deve ter no m�nimo 6 caracteres")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra mai�scula")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra min�scula")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um n�mero")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("A confirma��o de senha � obrigat�ria")
            .Equal(x => x.Password).WithMessage("As senhas n�o conferem");
    }
}
