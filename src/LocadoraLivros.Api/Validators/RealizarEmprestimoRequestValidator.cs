using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Emprestimo;

namespace LocadoraLivros.Api.Validators;

public class RealizarEmprestimoRequestValidator : AbstractValidator<RealizarEmprestimoRequest>
{
    public RealizarEmprestimoRequestValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Cliente inválido");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("O empréstimo deve ter pelo menos um item");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LivroId)
                .GreaterThan(0).WithMessage("Livro inválido");

            item.RuleFor(i => i.DiasEmprestimo)
                .GreaterThan(0).WithMessage("Dias de empréstimo deve ser maior que zero")
                .LessThanOrEqualTo(90).WithMessage("Empréstimo não pode exceder 90 dias");
        });
    }
}
