using FluentValidation;
using LocadoraLivros.Api.Models.Pagination;

namespace LocadoraLivros.Api.Validators;

public class PaginationParametersValidator : AbstractValidator<PaginationParameters>
{
    public PaginationParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("O número da página deve ser maior ou igual a 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser maior ou igual a 1")
            .LessThanOrEqualTo(100).WithMessage("O tamanho da página deve ser menor ou igual a 100");

        RuleFor(x => x.OrderDirection)
            .Must(x => x == "asc" || x == "desc")
            .When(x => !string.IsNullOrEmpty(x.OrderDirection))
            .WithMessage("A direção da ordenação deve ser 'asc' ou 'desc'");
    }
}
