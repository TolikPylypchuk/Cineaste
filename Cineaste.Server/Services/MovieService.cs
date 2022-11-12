namespace Cineaste.Server.Services;

[AutoConstructor]
[GenerateAutoInterface]
public sealed partial class MovieService : IMovieService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<MovieService> logger;

    public async Task<MovieModel> GetMovie(Id<Movie> id)
    {
        this.logger.LogDebug("Getting the movie with id: {Id}", id.Value);

        var movie = await this.FindMovie(id);
        return movie.ToMovieModel();
    }

    public async Task<MovieModel> CreateMovie(Validated<MovieRequest> request)
    {
        this.logger.LogDebug("Creating a new movie");

        var list = await this.FindList(request.Value.ListId);
        var kind = await this.FindKind(request.Value.KindId, list);

        var movie = request.ToMovie(Id.CreateNew<Movie>(), kind);

        list.AddMovie(movie);
        dbContext.Movies.Add(movie);

        await dbContext.SaveChangesAsync();

        return movie.ToMovieModel();
    }

    public async Task<MovieModel> UpdateMovie(Id<Movie> id, Validated<MovieRequest> request)
    {
        this.logger.LogDebug("Updating a movie with ID {Id}", id.Value);

        var movie = await this.FindMovie(id);
        var list = await this.FindList(request.Value.ListId);

        if (!list.Movies.Contains(movie))
        {
            throw this.MovieDoesNotBelongToList(id, list.Id);
        }

        var kind = await this.FindKind(request.Value.KindId, list);

        movie.Update(request, kind);

        await dbContext.SaveChangesAsync();

        return movie.ToMovieModel();
    }

    public async Task DeleteMovie(Id<Movie> id)
    {
        this.logger.LogDebug("Deleting the movie with id: {Id}", id.Value);

        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            throw this.NotFound(id);
        }

        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task<Movie> FindMovie(Id<Movie> id)
    {
        var movie = await this.dbContext.Movies
            .Include(movie => movie.Titles)
            .Include(movie => movie.Kind)
            .Include(movie => movie.Tags)
            .AsSplitQuery()
            .SingleOrDefaultAsync(movie => movie.Id == id);

        return movie is not null ? movie : throw this.NotFound(id);
    }

    private async Task<CineasteList> FindList(Guid id)
    {
        var listId = Id.Create<CineasteList>(id);

        var list = await this.dbContext.Lists
            .Include(list => list.Movies)
                .ThenInclude(movie => movie.Titles)
            .Include(list => list.Movies)
                .ThenInclude(movie => movie.Kind)
            .Include(list => list.Movies)
                .ThenInclude(movie => movie.Tags)
            .Include(list => list.MovieKinds)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId);

        if (list is null)
        {
            throw this.NotFound(listId);
        }

        return list;
    }

    private async Task<MovieKind> FindKind(Guid id, CineasteList list)
    {
        var kindId = Id.Create<MovieKind>(id);
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
        new NotFoundException(Resources.Movie, $"Could not find a movie with id {id.Value}")
            .WithProperty(id);

    private Exception NotFound(Id<CineasteList> id) =>
        new NotFoundException(Resources.List, $"Could not find a list with id {id.Value}")
            .WithProperty(id);

    private Exception NotFound(Id<MovieKind> id) =>
        new NotFoundException(Resources.MovieKind, $"Could not find a movie kind with id {id.Value}")
            .WithProperty(id);

    private Exception MovieDoesNotBelongToList(Id<Movie> movieId, Id<CineasteList> listId) =>
        new BadRequestException(
            $"{Resources.Movie}.WrongList",
            $"Movie with ID {movieId.Value} does not belong to list with ID {listId}")
            .WithProperty(movieId)
            .WithProperty(listId);

    private Exception KindDoesNotBelongToList(Id<MovieKind> kindId, Id<CineasteList> listId) =>
        new BadRequestException(
            $"{Resources.MovieKind}.WrongList",
            $"Movie kind with ID {kindId.Value} does not belong to list with ID {listId}")
            .WithProperty(kindId)
            .WithProperty(listId);
}
