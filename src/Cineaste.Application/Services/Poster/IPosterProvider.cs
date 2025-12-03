namespace Cineaste.Application.Services.Poster;

public interface IPosterProvider
{
    Task<StreamableContent> FetchPoster(Validated<PosterUrlRequest> request, CancellationToken token = default);

    Task<StreamableContent> FetchPoster(Validated<PosterImdbMediaRequest> request, CancellationToken token = default);

    Task<string> GetPosterUrl(Validated<PosterImdbMediaRequest> request, CancellationToken token = default);
}
