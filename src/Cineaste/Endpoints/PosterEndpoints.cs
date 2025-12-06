using Cineaste.Application.Services.Poster;

namespace Cineaste.Endpoints;

public static class PosterEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapPosterEndpoints()
        {
            var posters = endpoints.MapGroup("/posters")
                .RequireAuthorization()
                .WithTags("Posters");

            posters.MapGet("/imdb-media", FetchPosterFromImdbMedia)
                .Produces(StatusCodes.Status302Found)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
                .WithName(nameof(FetchPosterFromImdbMedia))
                .WithSummary("Get a poster URL from IMDb media");

            return posters;
        }
    }

    public static async Task<RedirectHttpResult> FetchPosterFromImdbMedia(
        string url,
        IPosterProvider posterProvider,
        CancellationToken token) =>
        TypedResults.Redirect(await posterProvider.GetPosterUrl(new PosterImdbMediaRequest(url).Validated(), token));
}
