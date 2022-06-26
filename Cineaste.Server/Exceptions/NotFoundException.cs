namespace Cineaste.Server.Exceptions;

public sealed class NotFoundException : CineasteException
{
    public string? Resource { get; }

    public NotFoundException(
        string messageCode,
        string? message = null,
        string? resource = null,
        IDictionary<string, object?>? properties = null,
        Exception? innerException = null)
        : base(messageCode, message, properties, innerException) =>
        this.Resource = resource;
}
