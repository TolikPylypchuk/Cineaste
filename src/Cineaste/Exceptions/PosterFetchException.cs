using System.Net.Http.Headers;

namespace Cineaste.Exceptions;

public sealed class PosterFetchException(
    string messageCode,
    string? message = null,
    Exception ? innerException = null)
    : CineasteException($"Poster.Fetch.{messageCode}", message, innerException)
{
    public static async Task<PosterFetchException> FromResponse(
        HttpResponseMessage response,
        CancellationToken token = default)
    {
        var responseData = new
        {
            StatusCode = (int)response.StatusCode,
            Headers = response.Headers.ToDictionary(),
            Body = await response.Content.ReadAsStringAsync(token)
        };

        throw new PosterFetchException("UnsuccessfulResponse", "Unsuccessful response when fetching a poster")
            .WithProperty("response", responseData);
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
