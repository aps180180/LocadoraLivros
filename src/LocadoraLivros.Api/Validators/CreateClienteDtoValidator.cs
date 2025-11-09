using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Cliente;
using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Validators;

public class CreateClienteDtoValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(3, 200).WithMessage("Nome deve ter entre 3 e 200 caracteres");

        RuleFor(x => x.CPF)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Length(11).WithMessage("CPF deve ter 11 dígitos")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter apenas números");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(100);

        RuleFor(x => x.Telefone)
            .MaximumLength(20);

        RuleFor(x => x.Celular)
            .MaximumLength(20);

        RuleFor(x => x.Estado)
            .Length(2).When(x => !string.IsNullOrEmpty(x.Estado))
            .WithMessage("Estado deve ter 2 caracteres");

        RuleFor(x => x.CEP)
            .MaximumLength(10);

        // ✅ ADICIONAR
        RuleFor(x => x.TipoCliente)
            .IsInEnum().WithMessage("Tipo de cliente inválido");
    }
}
