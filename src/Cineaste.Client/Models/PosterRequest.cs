namespace Cineaste.Client.Models;

public sealed record PosterRequest
{
    public IBrowserFile? File { get; }

    public PosterUrlRequest? UrlRequest { get; }

    public PosterRequest(IBrowserFile file) =>
        this.File = file ?? throw new ArgumentNullException(nameof(file));

    public PosterRequest(PosterUrlRequest urlRequest) =>
        this.UrlRequest = urlRequest ?? throw new ArgumentNullException(nameof(urlRequest));

    public T Select<T>(Func<IBrowserFile, T> fileMapper, Func<PosterUrlRequest, T> urlRequestMapper)
    {
        if (this.File is { } file)
        {
            return fileMapper(file);
        } else if (this.UrlRequest is { } urlRequest)
        {
            return urlRequestMapper(urlRequest);
        }

        throw new InvalidOperationException("The poster request must have either a file or a URL request set");
    }
}
