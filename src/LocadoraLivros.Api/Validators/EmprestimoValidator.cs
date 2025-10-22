using FluentValidation;
using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Shared.Enums;

namespace LocadoraLivros.Api.Validators;

public class EmprestimoValidator : AbstractValidator<Emprestimo>
{
    public EmprestimoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("O cliente deve ser informado");

        RuleFor(x => x.DataEmprestimo)
            .NotEmpty().WithMessage("A data de empréstimo é obrigatória");

        RuleFor(x => x.DataPrevisaoDevolucao)
            .NotEmpty().WithMessage("A data de previsão de devolução é obrigatória")
            .GreaterThanOrEqualTo(x => x.DataEmprestimo)
            .WithMessage("A data de previsão de devolução deve ser maior ou igual à data de empréstimo");

        RuleFor(x => x.DataDevolucao)
            .GreaterThanOrEqualTo(x => x.DataEmprestimo)
            .When(x => x.DataDevolucao.HasValue)
            .WithMessage("A data de devolução deve ser maior ou igual à data de empréstimo");

        RuleFor(x => x.ValorTotal)
            .GreaterThanOrEqualTo(0).WithMessage("O valor total não pode ser negativo");

        RuleFor(x => x.ValorMulta)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ValorMulta.HasValue)
            .WithMessage("O valor da multa não pode ser negativo");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status inválido"); 
        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observações devem ter no máximo 500 caracteres");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("O empréstimo deve ter pelo menos um item");
    }
}
