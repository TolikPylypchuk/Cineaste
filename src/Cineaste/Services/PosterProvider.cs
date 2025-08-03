using System.Net.Http.Headers;

namespace Cineaste.Services;

public interface IPosterProvider
{
    Task<BinaryContentModel> FetchPoster(Validated<PosterUrlRequest> request, CancellationToken token = default);
}

public sealed class PosterProvider(HttpClient httpClient, ILogger<PosterProvider> logger) : IPosterProvider
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<PosterProvider> logger = logger;

    public async Task<BinaryContentModel> FetchPoster(
        Validated<PosterUrlRequest> request,
        CancellationToken token = default)
    {
        try
        {
            this.logger.LogInformation("Fetching a poster from a remote URL: {Url}", request.Value.Url);

            var response = await this.httpClient.GetAsync(request.Value.Url, token);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogInformation(
                    "Unsuccessful response when fetching a poster from a remote URL: {Url}", request.Value.Url);

                var responseData = new
                {
                    StatusCode = (int)response.StatusCode,
                    Headers = response.Headers.ToDictionary(),
                    Body = await response.Content.ReadAsStringAsync(token)
                };

                throw new PosterFetchException("UnsuccessfulResponse", "Unsuccessful response when fetching a poster")
                    .WithProperty("response", responseData);
            }

            string contentType = response.Content.Headers.ContentType?.MediaType
                ?? throw new PosterFetchException("NoMediaType", "Fetched poster does not have a media type");

            var responseBody = await response.Content.ReadAsByteArrayAsync(token);

            this.logger.LogInformation("Fetched a poster from a remote URL: {Url}", request.Value.Url);
            return new(responseBody, contentType);
        } catch (PosterFetchException)
        {
            throw;
        } catch (OperationCanceledException)
        {
            throw;
        } catch (Exception e)
        {
            this.logger.LogError(e, "Exception when fetching a poster from a remote URL: {Url}", request.Value.Url);

            throw new PosterFetchException("Error", "Error fetching a poster", e);
        }
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
