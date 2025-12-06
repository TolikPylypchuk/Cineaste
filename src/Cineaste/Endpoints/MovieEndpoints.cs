using Cineaste.Application.Services.Poster;

namespace Cineaste.Endpoints;

public static class MovieEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapMovieEndpoints()
        {
            var movies = endpoints.MapGroup("/movies")
                .RequireAuthorization()
                .WithTags("Movies");

            movies.MapGet("/{id}", GetMovie)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(GetMovie))
                .WithSummary("Get a movie");

            movies.MapPost("/", AddMovie)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithName(nameof(AddMovie))
                .WithSummary("Add a movie to the list");
            
            movies.MapPut("/{id}", UpdateMovie)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithName(nameof(UpdateMovie))
                .WithSummary("Update a movie");

            movies.MapDelete("/{id}", RemoveMovie)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(RemoveMovie))
                .WithSummary("Remove a movie from the list");

            movies.MapGet("/{id}/poster", GetMoviePoster)
                .ProducesPosterContentTypes()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(GetMoviePoster))
                .WithSummary("Get a poster for a movie");

            movies.MapPut("/{id}/poster", SetMoviePoster)
                .AcceptsPosterContentTypes()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
                .WithName(nameof(SetMoviePoster))
                .WithSummary("Set a poster for a movie");

            movies.MapPut("/{id}/poster", SetIndirectMoviePoster)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
                .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
                .WithName(nameof(SetIndirectMoviePoster))
                .WithSummary("Set a poster for a movie");

            movies.MapDelete("/{id}/poster", RemoveMoviePoster)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(RemoveMoviePoster))
                .WithSummary("Remove a poster for a movie");

            return movies;
        }
    }

    public static async Task<Ok<MovieModel>> GetMovie(
        Guid id,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await movieService.GetMovie(principal.ListId, Id.For<Movie>(id), token));

    public static async Task<Created<MovieModel>> AddMovie(
        MovieRequest request,
        MovieService movieService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken token)
    {
        var movie = await movieService.AddMovie(httpContext.User.ListId, request.Validated(), token);
        var uri = linkGenerator.GetUriByName(httpContext, nameof(GetMovie), new { id = movie.Id });
        return TypedResults.Created(uri, movie);
    }

    public static async Task<Ok<MovieModel>> UpdateMovie(
        Guid id,
        MovieRequest request,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await movieService.UpdateMovie(
            principal.ListId, Id.For<Movie>(id), request.Validated(), token));

    public static async Task<NoContent> RemoveMovie(
        Guid id,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await movieService.RemoveMovie(principal.ListId, Id.For<Movie>(id), token);
        return TypedResults.NoContent();
    }

    public static async Task<FileContentHttpResult> GetMoviePoster(
        Guid id,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await movieService.GetMoviePoster(principal.ListId, Id.For<Movie>(id), token);
        return TypedResults.File(poster.Data, poster.Type);
    }

    public static async Task<Created> SetMoviePoster(
        Guid id,
        IFormFile file,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var movieId = Id.For<Movie>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await movieService.SetMoviePoster(principal.ListId, movieId, content, token);

        return TypedResults.Created(Urls.MoviePoster(movieId, posterHash));
    }

    public static async Task<Created> SetIndirectMoviePoster(
        Guid id,
        PosterRequestBase request,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var movieId = Id.For<Movie>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await movieService.SetMoviePoster(principal.ListId, movieId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await movieService.SetMoviePoster(principal.ListId, movieId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(Urls.MoviePoster(movieId, posterHash));
    }

    public static async Task<NoContent> RemoveMoviePoster(
        Guid id,
        MovieService movieService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await movieService.RemoveMoviePoster(principal.ListId, Id.For<Movie>(id), token);
        return TypedResults.NoContent();
    }
}
