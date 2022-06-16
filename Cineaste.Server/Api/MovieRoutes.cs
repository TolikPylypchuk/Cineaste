namespace Cineaste.Server.Api;

public static class MovieRoutes
{
    public static void MapMovieRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/movies/{id}", GetMovie);
        routes.MapDelete("/movies/{id}", DeleteMovie);
    }

    private static async Task<MovieModel> GetMovie(Guid id, IMovieService movieService) =>
        await movieService.GetMovie(new Id<Movie>(id));

    private static async Task<IResult> DeleteMovie(Guid id, IMovieService movieService)
    {
        await movieService.DeleteMovie(new Id<Movie>(id));
        return Results.NoContent();
    }
}
