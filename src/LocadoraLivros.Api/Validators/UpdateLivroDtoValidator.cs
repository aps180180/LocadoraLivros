using FluentValidation;
using LocadoraLivros.Api.Models.DTOs.Livro;

namespace LocadoraLivros.Api.Validators;

public class UpdateLivroDtoValidator : AbstractValidator<UpdateLivroDto>
{
    public UpdateLivroDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório")
            .Length(3, 200).WithMessage("Título deve ter entre 3 e 200 caracteres");

        RuleFor(x => x.Autor)
            .NotEmpty().WithMessage("Autor é obrigatório")
            .Length(3, 200).WithMessage("Autor deve ter entre 3 e 200 caracteres");

        RuleFor(x => x.Editora)
            .MaximumLength(100).WithMessage("Editora deve ter no máximo 100 caracteres");

        RuleFor(x => x.AnoPublicacao)
            .InclusiveBetween(1000, DateTime.Now.Year)
            .When(x => x.AnoPublicacao.HasValue)
            .WithMessage($"Ano de publicação deve estar entre 1000 e {DateTime.Now.Year}");

        RuleFor(x => x.Categoria)
            .MaximumLength(50).WithMessage("Categoria deve ter no máximo 50 caracteres");

        RuleFor(x => x.QuantidadeTotal)
            .GreaterThanOrEqualTo(0).WithMessage("Quantidade total deve ser maior ou igual a 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantidade total não pode exceder 1000");

        RuleFor(x => x.ValorDiaria)
            .GreaterThan(0).WithMessage("Valor da diária deve ser maior que zero")
            .LessThanOrEqualTo(1000).WithMessage("Valor da diária não pode exceder R$ 1.000,00");
    }
}
