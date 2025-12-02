namespace Cineaste.Application.Exceptions;

public sealed class InvalidInputException(
    string messageCode,
    string? message = null,
    Exception? innerException = null)
    : CineasteException($"BadRequest.{messageCode}", message, innerException)
{
}
