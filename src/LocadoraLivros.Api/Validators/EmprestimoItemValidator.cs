using FluentValidation;
using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Validators;

public class EmprestimoItemValidator : AbstractValidator<EmprestimoItem>
{
    public EmprestimoItemValidator()
    {
        RuleFor(x => x.LivroId)
            .GreaterThan(0).WithMessage("O livro deve ser informado");

        RuleFor(x => x.DiasEmprestimo)
            .GreaterThan(0).WithMessage("Os dias de empr�stimo devem ser maior que zero")
            .LessThanOrEqualTo(90).WithMessage("O empr�stimo n�o pode exceder 90 dias");

        RuleFor(x => x.ValorDiaria)
            .GreaterThan(0).WithMessage("O valor da di�ria deve ser maior que zero");

        RuleFor(x => x.ValorSubtotal)
            .GreaterThanOrEqualTo(0).WithMessage("O valor subtotal n�o pode ser negativo");

        RuleFor(x => x.ValorMultaItem)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ValorMultaItem.HasValue)
            .WithMessage("O valor da multa do item n�o pode ser negativo");
    }
}
