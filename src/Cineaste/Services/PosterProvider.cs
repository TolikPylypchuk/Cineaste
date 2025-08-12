using AngleSharp;
using AngleSharp.Dom;

using IConfiguration = AngleSharp.IConfiguration;

namespace Cineaste.Services;

public interface IPosterProvider
{
    Task<StreamableContent> FetchPoster(Validated<PosterUrlRequest> request, CancellationToken token = default);

    Task<StreamableContent> FetchPoster(Validated<PosterImdbMediaRequest> request, CancellationToken token = default);

    Task<string> GetPosterUrl(Validated<PosterImdbMediaRequest> request, CancellationToken token = default);
}

public sealed class PosterProvider(
    HttpClient httpClient,
    IConfiguration config,
    ILogger<PosterProvider> logger) : IPosterProvider
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IConfiguration config = config;
    private readonly ILogger<PosterProvider> logger = logger;

    public Task<StreamableContent> FetchPoster(
        Validated<PosterUrlRequest> request,
        CancellationToken token = default) =>
        this.FetchPoster(request.Value.Url, token);

    public async Task<StreamableContent> FetchPoster(
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token = default) =>
        await this.FetchPoster(await this.GetPosterUrl(request, token), token);

    public async Task<string> GetPosterUrl(Validated<PosterImdbMediaRequest> request, CancellationToken token)
    {
        string url = request.Value.Url;
        this.logger.LogInformation("Fetching a poster from IMDb: {Url}", url);

        var mediaId = this.GetMediaId(url);

        IElement? image;

        try
        {
            using var context = BrowsingContext.New(this.config);

            var document = await context.OpenAsync(url, token);
            image = document.QuerySelector($"img[data-image-id=\"{mediaId}-curr\"]");
        } catch (Exception e)
        {
            this.logger.LogError(e, "Exception when fetching IMDb media: {Url}", url);

            throw new PosterFetchException("Error", "Error fetching a poster", e);
        }

        if (image is { LocalName: "img" } && image.GetAttribute("src") is string source)
        {
            this.logger.LogInformation("Fetched image source URL from IMDb: {Source}", source);
            return source;
        } else
        {
            throw new InvalidInputException(
                "Imdb.Media.NotFound", "The image URL is not found in the IMDb media page")
                .WithProperty(url);
        }
    }

    private async Task<StreamableContent> FetchPoster(string url, CancellationToken token = default)
    {
        try
        {
            this.logger.LogInformation("Fetching a poster from a remote URL: {Url}", url);

            var response = await this.httpClient.GetAsync(url, token);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogInformation(
                    "Unsuccessful response when fetching a poster from a remote URL: {Url}", url);

                throw await PosterFetchException.FromResponse(response, token);
            }

            string contentType = response.Content.Headers.ContentType?.MediaType
                ?? throw new PosterFetchException("NoMediaType", "Fetched poster does not have a media type");

            long contentLength = response.Content.Headers.ContentLength
                ?? throw new PosterFetchException("NoLength", "Fetched poster does not have a length");

            this.ValidateContentType(contentType);

            var responseBody = await response.Content.ReadAsStreamAsync(token);

            this.logger.LogInformation("Fetched a poster from a remote URL: {Url}", url);
            return new StreamableContent(() => responseBody, contentLength, contentType);
        } catch (Exception e) when (e is CineasteException or OperationCanceledException)
        {
            throw;
        } catch (Exception e)
        {
            this.logger.LogError(e, "Exception when fetching a poster from a remote URL: {Url}", url);

            throw new PosterFetchException("Error", "Error fetching a poster", e);
        }
    }

    private string GetMediaId(string url) =>
        new Uri(url).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault()
            ?? throw new InvalidInputException("Imdb.Media.Invalid", "The IMDb media URL is invalid")
                .WithProperty(url);

    private void ValidateContentType(string contentType)
    {
        if (!PosterContentTypes.AcceptedImageContentTypes.Contains(contentType))
        {
            throw new UnsupportedPosterTypeException(contentType)
                .WithProperty(contentType);
        }
    }
}
