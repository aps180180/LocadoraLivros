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
            .NotEmpty().WithMessage("O empr�stimo deve ter pelo menos um item")
            .Must(itens => itens != null && itens.Count > 0 && itens.Count <= 10)
            .WithMessage("O empr�stimo deve ter entre 1 e 10 livros");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LivroId)
                .GreaterThan(0).WithMessage("Livro inv�lido");

            item.RuleFor(i => i.DiasEmprestimo)
                .GreaterThan(0).WithMessage("Dias de empr�stimo deve ser maior que zero")
                .LessThanOrEqualTo(90).WithMessage("O empr�stimo n�o pode exceder 90 dias");
        });

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("Observa��es devem ter no m�ximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        // Valida��o customizada: n�o pode emprestar o mesmo livro duas vezes no mesmo empr�stimo
        RuleFor(x => x.Itens)
            .Must(itens => itens == null || itens.Select(i => i.LivroId).Distinct().Count() == itens.Count)
            .WithMessage("N�o � poss�vel emprestar o mesmo livro mais de uma vez no mesmo empr�stimo");
    }
}
