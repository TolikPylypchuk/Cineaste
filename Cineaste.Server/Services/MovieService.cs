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

    public async Task<MovieModel> CreateMovie(Validated<MovieRequest> request)
    {
        var list = await this.FindList(request.Value.ListId);
        var kind = await this.FindKind(request.Value.KindId, list);

        var movie = request.ToMovie(Id.Create<Movie>(), kind);

        list.AddMovie(movie);
        dbContext.Movies.Add(movie);

        await dbContext.SaveChangesAsync();

        return movie.ToMovieModel();
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

    private async Task<CineasteList> FindList(Guid id)
    {
        var listId = new Id<CineasteList>(id);

        var list = await this.dbContext.Lists
            .Include(list => list.MovieKinds)
            .SingleOrDefaultAsync(list => list.Id == listId);

        if (list is null)
        {
            throw this.NotFound(listId);
        }

        return list;
    }

    private async Task<MovieKind> FindKind(Guid id, CineasteList list)
    {
        var kindId = new Id<MovieKind>(id);
        var kind = await this.dbContext.MovieKinds.FindAsync(kindId);

        if (kind is null)
        {
            throw this.NotFound(kindId);
        } else if (!list.MovieKinds.Contains(kind))
        {
            throw this.KindDoesNotBelongToList(kindId, list.Id);
        }

        return kind;
    }

    private Exception NotFound(Id<Movie> id) =>
        new NotFoundException(
            "NotFound.Movie",
            $"Could not find a movie with id {id.Value}",
            "Resource.Movie",
            new Dictionary<string, object?> { ["id"] = id });

    private Exception NotFound(Id<CineasteList> id) =>
        new NotFoundException(
            "NotFound.List",
            $"Could not find a list with id {id.Value}",
            "Resource.List",
            new Dictionary<string, object?> { ["id"] = id });

    private Exception NotFound(Id<MovieKind> id) =>
        new NotFoundException(
            "NotFound.MovieKind",
            $"Could not find a movie kind with id {id.Value}",
            "Resource.MovieKind",
            new Dictionary<string, object?> { ["id"] = id });

    private Exception KindDoesNotBelongToList(Id<MovieKind> kindId, Id<CineasteList> listId) =>
        new BadRequestException(
            "BadRequest.MovieKind.WrongList",
            $"Movie kind with ID {kindId.Value} does not belong to list with ID {listId}",
            new Dictionary<string, object?> { ["kindId"] = kindId, ["listId"] = listId });
}
