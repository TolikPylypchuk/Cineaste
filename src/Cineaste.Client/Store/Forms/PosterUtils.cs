namespace Cineaste.Client.Store.Forms;

public static class PosterUtils
{
    public static string GetPosterLocation(IApiResponse response) =>
        response.Headers.Location is { } location
            ? location.ToString()
            : throw new InvalidOperationException("The poster response must have the Location header");
}
