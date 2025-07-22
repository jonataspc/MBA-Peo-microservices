namespace Peo.Core.DomainObjects.Result;

public record Error(string? Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Um valor nulo foi fornecido.");

    public Error(string message) : this(default, message)
    {
        Message = message;
    }

    public Error() : this(default, default!)
    {
    }
}