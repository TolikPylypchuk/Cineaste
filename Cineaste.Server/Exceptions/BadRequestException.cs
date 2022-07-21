namespace Cineaste.Server.Exceptions;

public sealed class BadRequestException : CineasteException
{
    public BadRequestException(
        string messageCode,
        string? message = null,
        Exception? innerException = null)
        : base($"BadRequest.{messageCode}", message, innerException)
    { }
}
