namespace Cineaste.Server.Services;

[AutoConstructor]
public sealed partial class MovieService : IMovieService
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

    public async Task DeleteMovie(Id<Movie> id)
    {
        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            throw this.NotFound(id);
        }

        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync();
    }

    private Exception NotFound(Id<Movie> id) =>
        new NotFoundException(
            "NotFound.Movie",
            $"Could not find a movie with id {id.Value}",
            "Resource.Movie",
            new Dictionary<string, object?> { ["id"] = id });
}
