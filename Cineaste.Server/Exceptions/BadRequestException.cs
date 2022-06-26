namespace Cineaste.Server.Exceptions;

public sealed class BadRequestException : CineasteException
{
    public BadRequestException(
        string messageCode,
        string? message = null,
        IDictionary<string, object?>? properties = null,
        Exception? innerException = null)
        : base(messageCode, message, properties, innerException)
    { }
}
