namespace LocadoraLivros.Api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} com id '{key}' não foi encontrado(a)")
    {
    }
}
