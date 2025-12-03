using System.Net.Http.Headers;

using AngleSharp.Dom;

namespace Cineaste.Application.Services.Poster;

public sealed class PosterProvider(
    HttpClient httpClient,
    IHtmlDocumentProvider documentProvider,
    ILogger<PosterProvider> logger) : IPosterProvider
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IHtmlDocumentProvider documentProvider = documentProvider;
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

        var image = await this.GetImage(url, token);
        return this.ExtractSource(image, url);
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

                throw await this.ExceptionFromResponse(url, response, token);
            }

            return await this.GetContent(response, url, token);
        } catch (Exception e) when (e is PosterException or OperationCanceledException)
        {
            throw;
        } catch (Exception e)
        {
            this.logger.LogError(e, "Exception when fetching a poster from a remote URL: {Url}", url);

            throw new PosterFetchException(e);
        }
    }

    private async Task<StreamableContent> GetContent(HttpResponseMessage response, string url, CancellationToken token)
    {
        string contentType = response.Content.Headers.ContentType?.MediaType
            ?? throw new NoPosterContentTypeException(url);

        long contentLength = response.Content.Headers.ContentLength
            ?? throw new NoPosterContentLengthException(url);

        if (!PosterContentTypes.AcceptedImageContentTypes.Contains(contentType))
        {
            throw new UnsupportedPosterTypeException(contentType, url);
        }

        var responseBody = await response.Content.ReadAsStreamAsync(token);

        this.logger.LogInformation("Fetched a poster from a remote URL: {Url}", url);
        return new StreamableContent(() => responseBody, contentLength, contentType);
    }

    private async Task<IElement?> GetImage(string url, CancellationToken token)
    {
        try
        {
            var mediaId = this.GetMediaId(url);

            var document = await this.documentProvider.GetDocument(url, token);
            return document.QuerySelector($"img[data-image-id=\"{mediaId}-curr\"]");
        } catch (OperationCanceledException)
        {
            throw;
        } catch (Exception e)
        {
            this.logger.LogError(e, "Exception when fetching IMDb media: {Url}", url);

            throw new PosterFetchException(e);
        }
    }

    private string ExtractSource(IElement? image, string url)
    {
        if (image is { LocalName: "img" } && image.GetAttribute("src") is string source)
        {
            this.logger.LogInformation("Fetched image source URL from IMDb: {Source}", source);
            return source;
        } else
        {
            throw new ImdbMediaImageNotFoundException(url);
        }
    }

    private string GetMediaId(string url) =>
        new Uri(url).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();

    private async Task<PosterFetchResponseException> ExceptionFromResponse(
        string url,
        HttpResponseMessage response,
        CancellationToken token = default)
    {
        var responseData = new OriginalResponseDetails(
            (int)response.StatusCode,
            response.Headers.ToDictionary(),
            await response.Content.ReadAsStringAsync(token));

        throw new PosterFetchResponseException(url, responseData);
    }
}

file static class Extensions
{
    public static Dictionary<string, object> ToDictionary(this HttpResponseHeaders headers) =>
        headers.ToDictionary(header => header.Key, header => header.Value.ToStringOrList());

    private static object ToStringOrList(this IEnumerable<string> value)
    {
        var list = value.ToList();
        return list.Count == 1 ? list[0] : list;
    }
}
