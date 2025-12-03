namespace Cineaste.Application.Exceptions;

public abstract class PosterException(
    string message,
    Exception? innerException = null)
    : Exception(message, innerException);

public sealed class PosterFetchException(Exception? innerException = null)
    : PosterException("Error fetching a poster", innerException);

public sealed class PosterFetchResponseException(
    string url,
    OriginalResponseDetails response,
    Exception ? innerException = null)
    : PosterException($"Unsuccessful response when fetching a poster: {response.StatusCode}", innerException)
{
    public string Url { get; } = url;
    public OriginalResponseDetails Response { get; } = response;
}

public sealed class UnsupportedPosterTypeException(string? contentType, string url, Exception? innerException = null)
    : PosterException($"Media type {contentType} is not supported for posters", innerException)
{
    public string Url { get; } = url;
    public string? ContentType { get; } = contentType;
}

public sealed class NoPosterContentTypeException(string url, Exception? innerException = null)
    : PosterException("Fetched poster does not have a content type", innerException)
{
    public string Url { get; } = url;
}

public sealed class NoPosterContentLengthException(string url, Exception? innerException = null)
    : PosterException("Fetched poster does not have a length", innerException)
{
    public string Url { get; } = url;
}

public sealed class ImdbMediaImageNotFoundException(string url, Exception? innerException = null)
    : PosterException("The image URL is not found in the IMDb media page", innerException)
{
    public string Url { get; } = url;
}
