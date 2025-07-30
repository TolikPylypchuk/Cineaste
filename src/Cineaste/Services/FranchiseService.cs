namespace Cineaste.Services;

public sealed class FranchiseService(CineasteDbContext dbContext, ILogger<FranchiseService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<FranchiseService> logger = logger;

    public async Task<FranchiseModel> GetFranchise(Id<Franchise> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the franchise with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var franchise = await this.FindFranchise(list, id, token);

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> AddFranchise(Validated<FranchiseRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Adding a new franchise");

        var list = await this.FindList(token);
        var franchise = await this.MapToFranchise(request, list, token);
        this.EnsureAccessibleInList(franchise);

        list.AddFranchise(franchise);
        list.SortItems();

        this.dbContext.Franchises.Add(franchise);
        await this.dbContext.SaveChangesAsync(token);

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> UpdateFranchise(
        Id<Franchise> id,
        Validated<FranchiseRequest> request,
        CancellationToken token)
    {
        this.logger.LogDebug("Updating the franchise with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var franchise = await this.FindFranchise(list, id, token);

        var (movies, series, franchises) = await this.GetAllItems(request.Value, token);
        var movieKind = this.ResolveMovieKind(request.Value, list);
        var seriesKind = this.ResolveSeriesKind(request.Value, list);

        var result = franchise.Update(request, movieKind, seriesKind, movies, series, franchises);

        this.EnsureAccessibleInList(franchise);
        list.SortItems();

        this.dbContext.FranchiseItems.RemoveRange(result.RemovedItems);
        await this.dbContext.SaveChangesAsync(token);

        return franchise.ToFranchiseModel();
    }

    public async Task RemoveFranchise(Id<Franchise> id, CancellationToken token)
    {
        this.logger.LogDebug("Removing the franchise with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var franchise = await this.FindFranchise(list, id, token);

        this.dbContext.FranchiseItems.RemoveRange(franchise.Children);

        franchise.DetachAllChildren();
        list.RemoveFranchise(franchise);
        list.SortItems();

        this.dbContext.ListItems.Remove(franchise.ListItem!);
        this.dbContext.Franchises.Remove(franchise);
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Franchise> FindFranchise(CineasteList list, Id<Franchise> id, CancellationToken token)
    {
        var franchise = list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw this.NotFound(id);

        await this.dbContext.Entry(franchise)
            .Reference(f => f.MovieKind)
            .Query()
            .LoadAsync(token);

        await this.dbContext.Entry(franchise)
            .Reference(f => f.SeriesKind)
            .Query()
            .LoadAsync(token);

        await this.LoadFullFranchise(franchise, token);

        return franchise;
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
                    .ThenInclude(season => season.Periods)
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
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.FranchiseItem)
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.FranchiseItem)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.FranchiseItem)
            .AsSplitQuery()
            .LoadAsync(token);

        return list;
    }

    private async Task<Franchise> MapToFranchise(
        Validated<FranchiseRequest> request,
        CineasteList list,
        CancellationToken token)
    {
        var (movies, series, franchises) = await this.GetAllItems(request.Value, token);

        var movieKindId = Id.For<MovieKind>(request.Value.KindId);
        var seriesKindId = Id.For<SeriesKind>(request.Value.KindId);

        var movieKind = this.ResolveMovieKind(request.Value, list);
        var seriesKind = this.ResolveSeriesKind(request.Value, list);

        return request.ToFranchise(
            Id.Create<Franchise>(),
            movieKind,
            seriesKind,
            movies.ToDictionary(movie => movie.Id, movie => movie),
            series.ToDictionary(series => series.Id, series => series),
            franchises.ToDictionary(franchise => franchise.Id, franchise => franchise));
    }

    private MovieKind ResolveMovieKind(FranchiseRequest request, CineasteList list)
    {
        var movieKindId = Id.For<MovieKind>(request.KindId);
        return request.KindSource == FranchiseKindSource.Movie
            ? list.MovieKinds.FirstOrDefault(kind => kind.Id == movieKindId) ?? throw this.NotFound(movieKindId)
            : list.MovieKinds.First();
    }

    private SeriesKind ResolveSeriesKind(FranchiseRequest request, CineasteList list)
    {
        var seriesKindId = Id.For<SeriesKind>(request.KindId);
        return request.KindSource == FranchiseKindSource.Series
            ? list.SeriesKinds.FirstOrDefault(kind => kind.Id == seriesKindId) ?? throw this.NotFound(seriesKindId)
            : list.SeriesKinds.First();
    }

    private async Task<(List<Movie>, List<Series>, List<Franchise>)> GetAllItems(
        FranchiseRequest request,
        CancellationToken token)
    {
        var (movies, missingMovieIds) = await this.GetItems<Movie>(request, FranchiseItemType.Movie, token);
        var (series, missingSeriesIds) = await this.GetItems<Series>(request, FranchiseItemType.Series, token);
        var (franchises, missingFranchiseIds) = await this.GetItems<Franchise>(
            request, FranchiseItemType.Franchise, token);

        if (missingMovieIds.Any() || missingSeriesIds.Any() || missingFranchiseIds.Any())
        {
            throw this.NotFound(missingMovieIds, missingSeriesIds, missingFranchiseIds);
        }

        foreach (var s in series)
        {
            await this.LoadSeriesComponents(s, token);
        }

        foreach (var franchise in franchises)
        {
            await this.LoadFullFranchise(franchise, token);
        }

        return (movies, series, franchises);
    }

    private async Task<(List<T>, IReadOnlySet<Id<T>>)> GetItems<T>(
        FranchiseRequest request,
        FranchiseItemType itemType,
        CancellationToken token)
        where T : FranchiseItemEntity<T>
    {
        var ids = request.Items
            .Where(item => item.Type == itemType)
            .Select(item => Id.For<T>(item.Id))
            .ToImmutableHashSet();

        var items = ids.IsEmpty()
            ? []
            : await this.dbContext.Set<T>()
                .Where(item => ids.Contains(item.Id))
                .Include(item => item.FranchiseItem)
                .ToListAsync(token);

        var missingIds = ids.Except(items.Select(item => item.Id)).ToImmutableSortedSet();

        return (items, missingIds);
    }

    private async Task LoadSeriesComponents(Series series, CancellationToken token)
    {
        await this.dbContext.Entry(series)
            .Collection(s => s.Seasons)
            .Query()
            .Include(s => s.Periods)
            .Include(s => s.AllTitles)
            .LoadAsync(token);

        await this.dbContext.Entry(series)
            .Collection(s => s.SpecialEpisodes)
            .Query()
            .Include(e => e.AllTitles)
            .LoadAsync(token);
    }

    private void EnsureAccessibleInList(Franchise franchise)
    {
        if (franchise.Children.Count == 0)
        {
            franchise.ShowTitles = true;
        }
    }

    private NotFoundException ListNotFound() =>
        new(Resources.List, "Could not find the list");

    private CineasteException NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(
        IEnumerable<Id<Movie>> movieIds,
        IEnumerable<Id<Series>> seriesIds,
        IEnumerable<Id<Franchise>> franchiseIds) =>
        new NotFoundException(Resources.FranchiseItems, $"Could not find franchise items")
            .WithProperty(movieIds)
            .WithProperty(seriesIds)
            .WithProperty(franchiseIds);

    private CineasteException NotFound(Id<MovieKind> id) =>
        new NotFoundException(Resources.MovieKind, $"Could not find a movie kind with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<SeriesKind> id) =>
        new NotFoundException(Resources.SeriesKind, $"Could not find a series kind with ID {id.Value}")
            .WithProperty(id);
}
