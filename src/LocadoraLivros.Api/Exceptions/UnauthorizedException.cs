namespace LocadoraLivros.Api.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Acesso n�o autorizado")
    {
    }
}
