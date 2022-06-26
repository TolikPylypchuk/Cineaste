namespace Cineaste.Server.Exceptions;

public class CineasteException : Exception
{
    private const string DefaultMessageCode = "Unknown";

    public string MessageCode { get; }
    public IDictionary<string, object?>? Properties { get; }

    public CineasteException(
        string messageCode = DefaultMessageCode,
        string? message = null,
        IDictionary<string, object?>? properties = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        this.MessageCode = Require.NotNull(messageCode);
        this.Properties = properties;
    }
}
