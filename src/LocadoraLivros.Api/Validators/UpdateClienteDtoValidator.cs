using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Cliente;

namespace LocadoraLivros.Api.Validators;

public class UpdateClienteDtoValidator : AbstractValidator<UpdateClienteDto>
{
    public UpdateClienteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(3, 200).WithMessage("Nome deve ter entre 3 e 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(100);

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

        RuleFor(x => x.Celular)
            .MaximumLength(20).WithMessage("Celular deve ter no máximo 20 caracteres");

        RuleFor(x => x.Endereco)
            .MaximumLength(300).WithMessage("Endereço deve ter no máximo 300 caracteres");

        RuleFor(x => x.Cidade)
            .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

        RuleFor(x => x.Estado)
            .Length(2).When(x => !string.IsNullOrEmpty(x.Estado))
            .WithMessage("Estado deve ter 2 caracteres");

        RuleFor(x => x.CEP)
            .MaximumLength(10).WithMessage("CEP deve ter no máximo 10 caracteres");

        RuleFor(x => x.TipoCliente)
            .IsInEnum().WithMessage("Tipo de cliente inválido");
    }
}
