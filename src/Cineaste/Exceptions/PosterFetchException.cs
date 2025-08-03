namespace Cineaste.Exceptions;

public sealed class PosterFetchException(
    string messageCode,
    string? message = null,
    Exception ? innerException = null)
    : CineasteException($"Poster.Fetch.{messageCode}", message, innerException)
{
}
