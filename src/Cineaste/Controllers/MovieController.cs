using Microsoft.AspNetCore.Authorization;

using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Controllers;

[ApiController]
[Route("/api/movies")]
[Authorize]
[Tags(["Movies"])]
public sealed class MovieController(MovieService movieService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a movie")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieModel>> GetMovie(Guid id, CancellationToken token) =>
        this.Ok(await movieService.GetMovie(this.GetCurrentListId(), Id.For<Movie>(id), token));

    [HttpPost]
    [EndpointSummary("Add a movie to the list")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieModel>> AddMovie([FromBody] MovieRequest request, CancellationToken token)
    {
        var movie = await movieService.AddMovie(this.GetCurrentListId(), request.Validated(), token);
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
        this.Ok(await movieService.UpdateMovie(this.GetCurrentListId(), Id.For<Movie>(id), request.Validated(), token));

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a movie from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveMovie(Guid id, CancellationToken token)
    {
        await movieService.RemoveMovie(this.GetCurrentListId(), Id.For<Movie>(id), token);
        return this.NoContent();
    }

    [HttpGet("{id}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a movie")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetMoviePoster(Guid id, CancellationToken token)
    {
        var poster = await movieService.GetMoviePoster(this.GetCurrentListId(), Id.For<Movie>(id), token);
        return this.File(poster.Data, poster.Type);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a movie")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetMoviePoster(Guid id, IFormFile file, CancellationToken token)
    {
        var movieId = Id.For<Movie>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await movieService.SetMoviePoster(this.GetCurrentListId(), movieId, content, token);

        return this.Created(Urls.MoviePoster(movieId, posterHash), null);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a movie")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetMoviePoster(
        Guid id,
        [FromBody] PosterRequestBase request,
        CancellationToken token)
    {
        var movieId = Id.For<Movie>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await movieService.SetMoviePoster(this.GetCurrentListId(), movieId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await movieService.SetMoviePoster(
                    this.GetCurrentListId(), movieId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return this.Created(Urls.MoviePoster(movieId, posterHash), null);
    }

    [HttpDelete("{id}/poster")]
    [EndpointSummary("Remove a poster for a movie")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveMoviePoster(Guid id, CancellationToken token)
    {
        await movieService.RemoveMoviePoster(this.GetCurrentListId(), Id.For<Movie>(id), token);
        return this.NoContent();
    }

    private Id<CineasteList> GetCurrentListId() =>
        this.User.GetListId() ?? throw new InvalidOperationException("List ID not found in the current user's claims");
}
