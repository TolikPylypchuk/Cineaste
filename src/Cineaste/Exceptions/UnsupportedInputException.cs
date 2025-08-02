namespace Cineaste.Exceptions;

public sealed class UnsupportedInputException(
    string messageCode,
    string? message = null,
    Exception? innerException = null)
    : CineasteException($"UnsupportedInput.{messageCode}", message, innerException)
{
}
