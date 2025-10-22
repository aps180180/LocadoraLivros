namespace LocadoraLivros.Api.Shared.Enums;

public enum EmprestimoStatus
{
    Ativo = 1,
    Devolvido = 2,
    Atrasado = 3,
    Cancelado = 4
}

public static class EmprestimoStatusExtensions
{
    public static string ToDescricao(this EmprestimoStatus status)
    {
        return status switch
        {
            EmprestimoStatus.Ativo => "Ativo",
            EmprestimoStatus.Devolvido => "Devolvido",
            EmprestimoStatus.Atrasado => "Atrasado",
            EmprestimoStatus.Cancelado => "Cancelado",
            _ => throw new ArgumentException("Status inválido")
        };
    }

    public static bool IsAtivo(this EmprestimoStatus status)
    {
        return status == EmprestimoStatus.Ativo;
    }

    public static bool IsDevolvido(this EmprestimoStatus status)
    {
        return status == EmprestimoStatus.Devolvido;
    }
}
