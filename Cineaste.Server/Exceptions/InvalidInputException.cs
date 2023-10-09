namespace Cineaste.Server.Exceptions;

public sealed class InvalidInputException : CineasteException
{
    public InvalidInputException(
        string messageCode,
        string? message = null,
        Exception? innerException = null)
        : base($"BadRequest.{messageCode}", message, innerException)
    { }
}
