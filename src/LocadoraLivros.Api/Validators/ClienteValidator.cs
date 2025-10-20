using FluentValidation;
using LocadoraLivros.Api.Models;

namespace LocadoraLivros.Api.Validators;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome � obrigat�rio")
            .MinimumLength(3).WithMessage("O nome deve ter no m�nimo 3 caracteres")
            .MaximumLength(200).WithMessage("O nome deve ter no m�ximo 200 caracteres");

        RuleFor(x => x.CPF)
            .NotEmpty().WithMessage("O CPF � obrigat�rio")
            .Must(ValidarCPF).WithMessage("CPF inv�lido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail � obrigat�rio")
            .EmailAddress().WithMessage("E-mail inv�lido")
            .MaximumLength(150).WithMessage("O e-mail deve ter no m�ximo 150 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("O telefone � obrigat�rio")
            .Matches(@"^\(?\d{2}\)?[\s\-]?\d{4,5}\-?\d{4}$")
            .WithMessage("Telefone inv�lido. Formato esperado: (00) 0000-0000 ou (00) 00000-0000");

        RuleFor(x => x.Celular)
            .Matches(@"^\(?\d{2}\)?[\s\-]?\d{5}\-?\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Celular))
            .WithMessage("Celular inv�lido. Formato esperado: (00) 00000-0000");

        RuleFor(x => x.Endereco)
            .NotEmpty().WithMessage("O endere�o � obrigat�rio")
            .MaximumLength(300).WithMessage("O endere�o deve ter no m�ximo 300 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("A cidade � obrigat�ria")
            .MaximumLength(100).WithMessage("A cidade deve ter no m�ximo 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("O estado � obrigat�rio")
            .Length(2).WithMessage("O estado deve ter 2 caracteres (UF)")
            .Matches("^[A-Z]{2}$").WithMessage("Estado inv�lido. Use sigla em mai�sculo (ex: SP, RJ)");

        RuleFor(x => x.CEP)
            .NotEmpty().WithMessage("O CEP � obrigat�rio")
            .Matches(@"^\d{5}\-?\d{3}$").WithMessage("CEP inv�lido. Formato esperado: 00000-000");
    }

    private bool ValidarCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "").Trim();

        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf.Substring(0, 9);
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }
}
