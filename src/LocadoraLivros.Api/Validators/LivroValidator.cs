using FluentValidation;
using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Validators;

public class LivroValidator : AbstractValidator<Livro>
{
    public LivroValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O t�tulo � obrigat�rio")
            .MinimumLength(2).WithMessage("O t�tulo deve ter no m�nimo 2 caracteres")
            .MaximumLength(200).WithMessage("O t�tulo deve ter no m�ximo 200 caracteres");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("O ISBN � obrigat�rio")
            .Length(10, 20).WithMessage("O ISBN deve ter entre 10 e 20 caracteres")
            .Matches(@"^[0-9\-]+$").WithMessage("O ISBN deve conter apenas n�meros e h�fens");

        RuleFor(x => x.Autor)
            .NotEmpty().WithMessage("O autor � obrigat�rio")
            .MinimumLength(2).WithMessage("O autor deve ter no m�nimo 2 caracteres")
            .MaximumLength(150).WithMessage("O autor deve ter no m�ximo 150 caracteres");

        RuleFor(x => x.Editora)
            .MaximumLength(100).WithMessage("A editora deve ter no m�ximo 100 caracteres");

        RuleFor(x => x.AnoPublicacao)
            .GreaterThan(1450).WithMessage("O ano de publica��o deve ser maior que 1450 (inven��o da prensa)")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("O ano de publica��o n�o pode ser futuro");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("A categoria � obrigat�ria")
            .MaximumLength(50).WithMessage("A categoria deve ter no m�ximo 50 caracteres");

        RuleFor(x => x.QuantidadeTotal)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade total n�o pode ser negativa");

        RuleFor(x => x.QuantidadeDisponivel)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade dispon�vel n�o pode ser negativa")
            .LessThanOrEqualTo(x => x.QuantidadeTotal)
            .WithMessage("A quantidade dispon�vel n�o pode ser maior que a quantidade total");

        RuleFor(x => x.ValorDiaria)
            .GreaterThan(0).WithMessage("O valor da di�ria deve ser maior que zero")
            .LessThanOrEqualTo(1000).WithMessage("O valor da di�ria deve ser menor ou igual a R$ 1.000,00");

        RuleFor(x => x.ImagemUrl)
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no m�ximo 500 caracteres")
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Relative) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("URL da imagem inv�lida");
    }
}
