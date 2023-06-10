namespace Cineaste.Server.Api;

public static class MovieRoutes
{
    public static void MapMovieRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/movies/{id}", GetMovie);
        routes.MapPost("/api/movies", CreateMovie);
        routes.MapPut("/api/movies/{id}", UpdateMovie);
        routes.MapDelete("/api/movies/{id}", DeleteMovie);
    }

    private static async Task<IResult> GetMovie(Guid id, MovieService movieService) =>
        Results.Ok(await movieService.GetMovie(Id.For<Movie>(id)));

    private static async Task<IResult> CreateMovie(Validated<MovieRequest> request, MovieService movieService)
    {
        var movie = await movieService.CreateMovie(request);
        return Results.Created($"/api/movies/{movie.Id}", movie);
    }

    private static async Task<IResult> UpdateMovie(
        Guid id,
        Validated<MovieRequest> request,
        MovieService movieService) =>
        Results.Ok(await movieService.UpdateMovie(Id.For<Movie>(id), request));

    private static async Task<IResult> DeleteMovie(Guid id, MovieService movieService)
    {
        await movieService.DeleteMovie(Id.For<Movie>(id));
        return Results.NoContent();
    }
}
