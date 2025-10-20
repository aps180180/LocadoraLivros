using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;

namespace LocadoraLivros.Api.Validators;

public class RealizarEmprestimoRequestValidator : AbstractValidator<RealizarEmprestimoRequest>
{
    public RealizarEmprestimoRequestValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Cliente inv�lido");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("O empr�stimo deve ter pelo menos um item");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LivroId)
                .GreaterThan(0).WithMessage("Livro inv�lido");

            item.RuleFor(i => i.DiasEmprestimo)
                .GreaterThan(0).WithMessage("Dias de empr�stimo deve ser maior que zero")
                .LessThanOrEqualTo(90).WithMessage("Empr�stimo n�o pode exceder 90 dias");
        });
    }
}
