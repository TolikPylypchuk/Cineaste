namespace Cineaste.Client.Models;

public sealed record PosterRequest
{
    public IBrowserFile? File { get; }

    public PosterRequestBase? Request { get; }

    public PosterRequest(IBrowserFile file) =>
        this.File = file ?? throw new ArgumentNullException(nameof(file));

    public PosterRequest(PosterUrlRequest urlRequest) =>
        this.Request = urlRequest ?? throw new ArgumentNullException(nameof(urlRequest));

    public PosterRequest(PosterImdbMediaRequest imdbMediaRequest) =>
        this.Request = imdbMediaRequest ?? throw new ArgumentNullException(nameof(imdbMediaRequest));

    public T Select<T>(Func<IBrowserFile, T> fileMapper, Func<PosterRequestBase, T> urlRequestMapper)
    {
        if (this.File is { } file)
        {
            return fileMapper(file);
        } else if (this.Request is { } request)
        {
            return urlRequestMapper(request);
        }

        throw new InvalidOperationException("The poster request has none of its fields set");
    }
}
