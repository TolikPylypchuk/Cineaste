namespace Cineaste.Controllers;

[ApiController]
[Route("/api/posters")]
[Tags(["Posters"])]
public sealed class PosterController(IPosterProvider posterProvider) : ControllerBase
{
    [HttpGet("imdb-media")]
    [EndpointSummary("Get a poster URL from IMDb media")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> FetchPosterFromImdbMedia([FromQuery] string url, CancellationToken token) =>
        this.Redirect(await posterProvider.GetPosterUrl(new PosterImdbMediaRequest(url).Validated(), token));
}
