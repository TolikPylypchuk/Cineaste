namespace Cineaste.Server.Exceptions;

public class CineasteException : Exception
{
    private const string DefaultMessageCode = "Unknown";

    public string MessageCode { get; }

    public CineasteException(
        string messageCode = DefaultMessageCode,
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException) =>
        this.MessageCode = Require.NotNull(messageCode);
}
