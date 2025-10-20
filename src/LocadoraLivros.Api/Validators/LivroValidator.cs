using FluentValidation;
using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Validators;

public class LivroValidator : AbstractValidator<Livro>
{
    public LivroValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório")
            .MinimumLength(2).WithMessage("O título deve ter no mínimo 2 caracteres")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("O ISBN é obrigatório")
            .Length(10, 20).WithMessage("O ISBN deve ter entre 10 e 20 caracteres")
            .Matches(@"^[0-9\-]+$").WithMessage("O ISBN deve conter apenas números e hífens");

        RuleFor(x => x.Autor)
            .NotEmpty().WithMessage("O autor é obrigatório")
            .MinimumLength(2).WithMessage("O autor deve ter no mínimo 2 caracteres")
            .MaximumLength(150).WithMessage("O autor deve ter no máximo 150 caracteres");

        RuleFor(x => x.Editora)
            .MaximumLength(100).WithMessage("A editora deve ter no máximo 100 caracteres");

        RuleFor(x => x.AnoPublicacao)
            .GreaterThan(1450).WithMessage("O ano de publicação deve ser maior que 1450 (invenção da prensa)")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("O ano de publicação não pode ser futuro");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("A categoria é obrigatória")
            .MaximumLength(50).WithMessage("A categoria deve ter no máximo 50 caracteres");

        RuleFor(x => x.QuantidadeTotal)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade total não pode ser negativa");

        RuleFor(x => x.QuantidadeDisponivel)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade disponível não pode ser negativa")
            .LessThanOrEqualTo(x => x.QuantidadeTotal)
            .WithMessage("A quantidade disponível não pode ser maior que a quantidade total");

        RuleFor(x => x.ValorDiaria)
            .GreaterThan(0).WithMessage("O valor da diária deve ser maior que zero")
            .LessThanOrEqualTo(1000).WithMessage("O valor da diária deve ser menor ou igual a R$ 1.000,00");

        RuleFor(x => x.ImagemUrl)
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no máximo 500 caracteres")
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Relative) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("URL da imagem inválida");
    }
}
