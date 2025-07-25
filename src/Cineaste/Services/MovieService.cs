namespace Cineaste.Services;

public sealed class MovieService(CineasteDbContext dbContext, ILogger<MovieService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<MovieService> logger = logger;

    public async Task<MovieModel> GetMovie(Id<Movie> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);
        return movie.ToMovieModel();
    }

    public async Task<MovieModel> AddMovie(Validated<MovieRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Creating a new movie");

        var list = await this.FindList(token);
        var kind = this.FindKind(list, Id.For<MovieKind>(request.Value.KindId));

        var movie = request.ToMovie(Id.Create<Movie>(), kind);

        list.AddMovie(movie);
        this.dbContext.Movies.Add(movie);

        list.SortItems();

        await this.dbContext.SaveChangesAsync(token);

        return movie.ToMovieModel();
    }

    public async Task<MovieModel> UpdateMovie(Id<Movie> id, Validated<MovieRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Updating the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);

        if (!list.ContainsMovie(movie))
        {
            throw this.NotFound(id);
        }

        var kind = this.FindKind(list, Id.For<MovieKind>(request.Value.KindId));

        movie.Update(request, kind);
        movie.ListItem?.SetProperties(movie);

        list.SortItems();

        await this.dbContext.SaveChangesAsync(token);

        return movie.ToMovieModel();
    }

    public async Task RemoveMovie(Id<Movie> id, CancellationToken token)
    {
        this.logger.LogDebug("Deleting the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);

        if (movie.FranchiseItem is { } item)
        {
            item.ParentFranchise.RemoveMovie(movie);
        }

        list.RemoveItem(movie.ListItem!);
        list.SortItems();

        this.dbContext.ListItems.Remove(movie.ListItem!);
        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Movie> FindMovie(CineasteList list, Id<Movie> id, CancellationToken token)
    {
        var movie = list.Items
            .Select(item => item.Movie)
            .WhereNotNull()
            .FirstOrDefault(movie => movie.Id == id)
            ?? throw this.NotFound(id);

        await this.dbContext.Entry(movie)
            .Collection(m => m.Tags)
            .LoadAsync(token);

        await this.dbContext.Entry(movie)
            .Reference(m => m.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (movie.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(movie.FranchiseItem.ParentFranchise, token);
        }

        return movie;
    }

    private async Task LoadFullFranchise(Franchise franchise, CancellationToken token)
    {
        await this.dbContext.Entry(franchise)
            .Reference(f => f.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (franchise.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(franchise.FranchiseItem.ParentFranchise, token);
        } else
        {
            await this.LoadChildren(franchise, token);
        }
    }

    private async Task LoadChildren(Franchise franchise, CancellationToken token)
    {
        await this.dbContext.Entry(franchise)
            .Collection(f => f.Children)
            .Query()
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(f => f!.AllTitles)
            .LoadAsync(token);

        foreach (var childFranchise in franchise.Children.Select(item => item.Franchise).WhereNotNull())
        {
            await this.LoadChildren(childFranchise, token);
        }
    }

    private async Task<CineasteList> FindList(CancellationToken token)
    {
        var list = await this.dbContext.Lists.FirstOrDefaultAsync(token) ?? throw this.ListNotFound();

        await this.dbContext.Entry(list)
            .Reference(list => list.Configuration)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.MovieKinds)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.SeriesKinds)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.Items)
            .Query()
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.AllTitles)
            .LoadAsync(token);

        return list;
    }

    private MovieKind FindKind(CineasteList list, Id<MovieKind> id) =>
        list.MovieKinds
            .FirstOrDefault(kind => kind.Id == id)
            ?? throw this.NotFound(id);

    private NotFoundException ListNotFound() =>
        new(Resources.List, "Could not find the list");

    private CineasteException NotFound(Id<Movie> id) =>
        new NotFoundException(Resources.Movie, $"Could not find a movie with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<MovieKind> id) =>
        new NotFoundException(Resources.MovieKind, $"Could not find a movie kind with ID {id.Value}")
            .WithProperty(id);
}
