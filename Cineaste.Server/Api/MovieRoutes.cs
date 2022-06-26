namespace Cineaste.Server.Api;

public static class MovieRoutes
{
    public static void MapMovieRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/movies/{id}", GetMovie);
        routes.MapPost("/movies", CreateMovie);
        routes.MapDelete("/movies/{id}", DeleteMovie);
    }

    private static async Task<IResult> GetMovie(Guid id, IMovieService movieService) =>
        Results.Ok(await movieService.GetMovie(new Id<Movie>(id)));

    private static async Task<IResult> CreateMovie(Validated<MovieRequest> request, IMovieService movieService)
    {
        var movie = await movieService.CreateMovie(request);
        return Results.Created($"/api/movies/{movie.Id}", movie);
    }

    private static async Task<IResult> DeleteMovie(Guid id, IMovieService movieService)
    {
        await movieService.DeleteMovie(new Id<Movie>(id));
        return Results.NoContent();
    }
}
