using System.Runtime.CompilerServices;

namespace Cineaste.Application.Exceptions;

public class CineasteException(
    string messageCode = CineasteException.DefaultMessageCode,
    string? message = null,
    Exception? innerException = null) : Exception(message, innerException)
{
    private const string DefaultMessageCode = "Unknown";

    public string MessageCode { get; } = Require.NotNull(messageCode);
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

    public CineasteException WithProperty(string name, object? value)
    {
        this.Properties[name] = value;
        return this;
    }

    public CineasteException WithProperty(object? value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        this.Properties[name] = value;
        return this;
    }
}
