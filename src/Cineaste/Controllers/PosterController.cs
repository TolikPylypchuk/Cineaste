namespace Cineaste.Controllers;

[ApiController]
[Route("/api/posters")]
[Tags(["Franchises"])]
public sealed class PosterController(IPosterProvider posterProvider) : ControllerBase
{
    [HttpGet("imdb-media")]
    [EndpointSummary("Fetch a poster from IMDb media")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> FetchPosterFromImdbMedia([FromQuery] string url, CancellationToken token)
    {
        var posterContent = await posterProvider.FetchPoster(new PosterImdbMediaRequest(url).Validated(), token);
        return this.File(posterContent.GetStream(), posterContent.Type);
    }
}
