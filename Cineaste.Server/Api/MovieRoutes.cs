namespace Cineaste.Server.Api;

public static class MovieRoutes
{
    public static void MapMovieRoutes(this IEndpointRouteBuilder routes, PathString prefix)
    {
        routes.MapGet(prefix + "/movies/{id}", GetMovie);
        routes.MapPost(prefix + "/movies", CreateMovie);
        routes.MapPut(prefix + "/movies/{id}", UpdateMovie);
        routes.MapDelete(prefix + "/movies/{id}", DeleteMovie);
    }

    private static async Task<IResult> GetMovie(Guid id, IMovieService movieService) =>
        Results.Ok(await movieService.GetMovie(Id.Create<Movie>(id)));

    private static async Task<IResult> CreateMovie(Validated<MovieRequest> request, IMovieService movieService)
    {
        var movie = await movieService.CreateMovie(request);
        return Results.Created($"/api/movies/{movie.Id}", movie);
    }

    private static async Task<IResult> UpdateMovie(
        Guid id,
        Validated<MovieRequest> request,
        IMovieService movieService) =>
        Results.Ok(await movieService.UpdateMovie(Id.Create<Movie>(id), request));

    private static async Task<IResult> DeleteMovie(Guid id, IMovieService movieService)
    {
        await movieService.DeleteMovie(Id.Create<Movie>(id));
        return Results.NoContent();
    }
}
