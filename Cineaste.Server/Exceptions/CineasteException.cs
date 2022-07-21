namespace Cineaste.Server.Exceptions;

using System.Runtime.CompilerServices;

public class CineasteException : Exception
{
    private const string DefaultMessageCode = "Unknown";

    public string MessageCode { get; }
    public IDictionary<string, object?> Properties { get; }

    public CineasteException(
        string messageCode = DefaultMessageCode,
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        this.MessageCode = Require.NotNull(messageCode);
        this.Properties = new Dictionary<string, object?>();
    }

    public CineasteException WithProperty(string name, object? value)
    {
        this.Properties[name] = value;
        return this;
    }

    public CineasteException WithProperty(object? value, [CallerArgumentExpression("value")] string name = "")
    {
        this.Properties[name] = value;
        return this;
    }
}
