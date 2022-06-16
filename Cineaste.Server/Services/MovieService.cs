namespace Cineaste.Server.Services;

[AutoConstructor]
public sealed partial class MovieService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<MovieService> logger;

    public async Task<MovieModel> GetMovie(Id<Movie> id)
    {
        this.logger.LogDebug("Getting the movie with id: {ID}", id.Value);

        var movie = await this.dbContext.Movies
            .Include(movie => movie.Titles)
            .Include(movie => movie.Kind)
            .Include(movie => movie.Tags)
            .SingleOrDefaultAsync(movie => movie.Id == id);

        return movie is not null
            ? movie.ToMovieModel()
            : throw this.NotFound(id);
    }

    private Exception NotFound(Id<Movie> id) =>
        new NotFoundException(
            "NotFound.Movie",
            $"Could not find a movie with id {id.Value}",
            "Resource.Movie",
            new Dictionary<string, object?> { ["id"] = id });
}
