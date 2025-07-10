namespace Cineaste.Server.Controllers;

[ApiController]
[Route("/api/movies")]
[Tags(["Movies"])]
public sealed class MovieController(MovieService movieService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a movie")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieModel>> GetMovie([FromRoute] Guid id) =>
        this.Ok(await movieService.GetMovie(Id.For<Movie>(id)));

    [HttpPost]
    [EndpointSummary("Add a movie to the list")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieModel>> AddMovie([FromBody] MovieRequest request)
    {
        var movie = await movieService.CreateMovie(request.Validated());
        return this.Created($"/api/movies/{movie.Id}", movie);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a movie")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieModel>> UpdateMovie(
        [FromRoute] Guid id,
        [FromBody] MovieRequest request)
    {
        var movie = await movieService.UpdateMovie(Id.For<Movie>(id), request.Validated());
        return this.Ok(movie);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a movie from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveMovie([FromRoute] Guid id)
    {
        await movieService.DeleteMovie(Id.For<Movie>(id));
        return this.NoContent();
    }
}
