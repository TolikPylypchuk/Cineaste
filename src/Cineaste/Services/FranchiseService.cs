namespace Cineaste.Services;

public sealed class FranchiseService(CineasteDbContext dbContext, ILogger<FranchiseService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<FranchiseService> logger = logger;

    public async Task<FranchiseModel> GetFranchise(Id<Franchise> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the franchise with ID: {Id}", id.Value);

        var franchise = await this.FindFranchise(id, token);
        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> AddFranchise(Validated<FranchiseRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Creating a new franchise");

        var list = await this.FindList(request.Value.ListId, token);
        var franchise = await this.MapToFranchise(request, list, token);
        this.EnsureAccessibleInList(franchise);

        list.AddFranchise(franchise);
        list.SortItems();

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(token);

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> UpdateFranchise(
        Id<Franchise> id,
        Validated<FranchiseRequest> request,
        CancellationToken token)
    {
        this.logger.LogDebug("Updating the franchise with ID: {Id}", id.Value);

        var franchise = await this.FindFranchise(id, token);
        var list = await this.FindList(request.Value.ListId, token);

        if (!list.ContainsFranchise(franchise))
        {
            throw this.FranchiseDoesNotBelongToList(id, list.Id);
        }

        var (movies, series, franchises) = await this.GetAllItems(request.Value, token);
        var movieKind = this.ResolveMovieKind(request.Value, list);
        var seriesKind = this.ResolveSeriesKind(request.Value, list);

        var result = franchise.Update(request, movieKind, seriesKind, movies, series, franchises);

        this.EnsureAccessibleInList(franchise);
        list.SortItems();

        dbContext.FranchiseItems.RemoveRange(result.RemovedItems);
        await dbContext.SaveChangesAsync(token);

        return franchise.ToFranchiseModel();
    }

    public async Task RemoveFranchise(Id<Franchise> id, CancellationToken token)
    {
        this.logger.LogDebug("Deleting the franchise with ID: {Id}", id.Value);

        var franchise = await this.dbContext.Franchises
            .Where(franchise => franchise.Id == id)
            .Include(franchise => franchise.ListItem)
            .SingleOrDefaultAsync(token)
            ?? throw this.NotFound(id);

        var listItem = franchise.ListItem!;

        var list = await this.FindList(listItem.List.Id.Value, token);
        list.RemoveItem(listItem);
        list.SortItems();

        dbContext.ListItems.Remove(listItem);

        this.dbContext.Franchises.Remove(franchise);
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Franchise> FindFranchise(Id<Franchise> id, CancellationToken token)
    {
        var franchise = await this.dbContext.Franchises
            .Include(franchise => franchise.AllTitles)
            .Include(franchise => franchise.MovieKind)
            .Include(franchise => franchise.SeriesKind)
            .Include(franchise => franchise.FranchiseItem)
                .ThenInclude(item => item!.ParentFranchise)
                    .ThenInclude(franchise => franchise.Children)
            .AsSplitQuery()
            .SingleOrDefaultAsync(franchise => franchise.Id == id, token)
            ?? throw this.NotFound(id);

        await this.LoadAllChildren(franchise, token);

        return franchise;
    }

    private async Task LoadAllChildren(Franchise franchise, CancellationToken token)
    {
        var children = await dbContext.Entry(franchise)
            .Collection(franchise => franchise.Children)
            .Query()
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.AllTitles)
            .Include(item => item.Series!)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series!.Seasons)
                .ThenInclude(season => season!.Periods)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.AllTitles)
            .AsSplitQuery()
            .ToListAsync(token);

        foreach (var childFranchise in children.Select(item => item.Franchise).WhereNotNull())
        {
            await this.LoadAllChildren(childFranchise, token);
        }
    }

    private async Task<CineasteList> FindList(Guid id, CancellationToken token)
    {
        var listId = Id.For<CineasteList>(id);

        return await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .Include(list => list.Items)
                .ThenInclude(item => item.Franchise)
                    .ThenInclude(franchise => franchise!.AllTitles)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.NotFound(listId);
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

    private void EnsureAccessibleInList(Franchise franchise)
    {
        if (franchise.Children.Count == 0)
        {
            franchise.ShowTitles = true;
        }
    }

    private CineasteException NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<CineasteList> id) =>
        new NotFoundException(Resources.List, $"Could not find a list with ID {id.Value}")
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

    private CineasteException FranchiseDoesNotBelongToList(Id<Franchise> franchiseId, Id<CineasteList> listId) =>
        new InvalidInputException(
            $"{Resources.Franchise}.WrongList",
            $"Franchise with ID {franchiseId.Value} does not belong to list with ID {listId}")
            .WithProperty(franchiseId)
            .WithProperty(listId);
}
