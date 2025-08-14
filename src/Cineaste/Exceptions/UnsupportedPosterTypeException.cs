namespace Cineaste.Exceptions;

public sealed class UnsupportedPosterTypeException(string contentType, Exception? innerException = null)
    : CineasteException(
        $"Poster.ContentType.Unsupported", $"Media type {contentType} is not supported for posters", innerException);
