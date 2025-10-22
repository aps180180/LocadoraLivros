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
            .NotEmpty().WithMessage("O empréstimo deve ter pelo menos um item")
            .Must(itens => itens != null && itens.Count > 0 && itens.Count <= 10)
            .WithMessage("O empréstimo deve ter entre 1 e 10 livros");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LivroId)
                .GreaterThan(0).WithMessage("Livro inválido");

            item.RuleFor(i => i.DiasEmprestimo)
                .GreaterThan(0).WithMessage("Dias de empréstimo deve ser maior que zero")
                .LessThanOrEqualTo(90).WithMessage("O empréstimo não pode exceder 90 dias");
        });

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("Observações devem ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        // Validação customizada: não pode emprestar o mesmo livro duas vezes no mesmo empréstimo
        RuleFor(x => x.Itens)
            .Must(itens => itens == null || itens.Select(i => i.LivroId).Distinct().Count() == itens.Count)
            .WithMessage("Não é possível emprestar o mesmo livro mais de uma vez no mesmo empréstimo");
    }
}
