namespace Cineaste.Controllers;

[ApiController]
[Route("/api/movies")]
[Tags(["Movies"])]
public sealed class MovieController(MovieService movieService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a movie")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieModel>> GetMovie(Guid id, CancellationToken token) =>
        this.Ok(await movieService.GetMovie(Id.For<Movie>(id), token));

    [HttpPost]
    [EndpointSummary("Add a movie to the list")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieModel>> AddMovie([FromBody] MovieRequest request, CancellationToken token)
    {
        var movie = await movieService.AddMovie(request.Validated(), token);
        return this.Created($"/api/movies/{movie.Id}", movie);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a movie")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieModel>> UpdateMovie(
        Guid id,
        [FromBody] MovieRequest request,
        CancellationToken token) =>
        this.Ok(await movieService.UpdateMovie(Id.For<Movie>(id), request.Validated(), token));

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a movie from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveMovie(Guid id, CancellationToken token)
    {
        await movieService.RemoveMovie(Id.For<Movie>(id), token);
        return this.NoContent();
    }
}
