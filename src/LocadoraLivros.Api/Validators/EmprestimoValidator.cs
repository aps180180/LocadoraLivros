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
            .NotEmpty().WithMessage("A data de empr�stimo � obrigat�ria");

        RuleFor(x => x.DataPrevisaoDevolucao)
            .NotEmpty().WithMessage("A data de previs�o de devolu��o � obrigat�ria")
            .GreaterThanOrEqualTo(x => x.DataEmprestimo)
            .WithMessage("A data de previs�o de devolu��o deve ser maior ou igual � data de empr�stimo");

        RuleFor(x => x.DataDevolucao)
            .GreaterThanOrEqualTo(x => x.DataEmprestimo)
            .When(x => x.DataDevolucao.HasValue)
            .WithMessage("A data de devolu��o deve ser maior ou igual � data de empr�stimo");

        RuleFor(x => x.ValorTotal)
            .GreaterThanOrEqualTo(0).WithMessage("O valor total n�o pode ser negativo");

        RuleFor(x => x.ValorMulta)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ValorMulta.HasValue)
            .WithMessage("O valor da multa n�o pode ser negativo");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status inv�lido"); 
        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observa��es devem ter no m�ximo 500 caracteres");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("O empr�stimo deve ter pelo menos um item");
    }
}
