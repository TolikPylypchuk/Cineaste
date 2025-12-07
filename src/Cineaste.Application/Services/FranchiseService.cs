namespace Cineaste.Application.Services;

public sealed partial class FranchiseService(
    CineasteDbContext dbContext,
    IPosterProvider posterProvider,
    ILogger<FranchiseService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IPosterProvider posterProvider = posterProvider;
    private readonly ILogger<FranchiseService> logger = logger;

    public async Task<FranchiseModel> GetFranchise(Id<CineasteList> listId, Id<Franchise> id, CancellationToken token)
    {
        this.LogGetFranchise(id, listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.FindFranchise(list, id, token);

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> AddFranchise(
        Id<CineasteList> listId,
        Validated<FranchiseRequest> request,
        CancellationToken token)
    {
        this.LogAddFranchise(listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.MapToFranchise(request, list, token);
        this.EnsureAccessibleInList(franchise);

        if (request.Value.ParentFranchiseId is Guid franchiseId)
        {
            var parentFranchise = await this.FindFranchise(list, Id.For<Franchise>(franchiseId), token);
            var franchiseItem = parentFranchise.AttachFranchise(franchise, true);
            this.dbContext.FranchiseItems.Add(franchiseItem);
        }

        list.AddFranchise(franchise);
        list.SortItems();

        this.dbContext.Franchises.Add(franchise);
        await this.dbContext.SaveChangesAsync(token);

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> UpdateFranchise(
        Id<CineasteList> listId,
        Id<Franchise> id,
        Validated<FranchiseRequest> request,
        CancellationToken token)
    {
        this.LogUpdateFranchise(id, listId);

        var list = await this.FindList(listId, token);
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

    public async Task RemoveFranchise(Id<CineasteList> listId, Id<Franchise> id, CancellationToken token)
    {
        this.LogRemoveFranchise(id, listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.FindFranchise(list, id, token);

        this.dbContext.FranchiseItems.RemoveRange(franchise.Children);

        franchise.DetachAllChildren();
        list.RemoveFranchise(franchise);
        list.SortItems();

        this.dbContext.ListItems.Remove(franchise.ListItem!);
        this.dbContext.Franchises.Remove(franchise);
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<BinaryContent> GetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        CancellationToken token)
    {
        this.LogGetFranchisePoster(franchiseId, listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.FindFranchise(list, franchiseId, token);

        var poster = await this.dbContext.FranchisePosters
            .Where(poster => poster.Franchise == franchise)
            .FirstOrDefaultAsync(token)
            ?? throw new FranchisePosterNotFoundException(franchiseId);

        return poster.ToPosterModel();
    }

    public async Task<PosterHash> SetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        StreamableContent content,
        CancellationToken token) =>
        await this.SetFranchisePoster(listId, franchiseId, () => Task.FromResult(content), token);

    public async Task<PosterHash> SetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        await this.SetFranchisePoster(
            listId, franchiseId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task<PosterHash> SetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        await this.SetFranchisePoster(
            listId, franchiseId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveFranchisePoster(Id<CineasteList> listId, Id<Franchise> franchiseId, CancellationToken token)
    {
        this.LogRemoveFranchisePoster(franchiseId, listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.FindFranchise(list, franchiseId, token);

        var poster = await this.dbContext.FranchisePosters
            .Where(poster => poster.Franchise == franchise)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.FranchisePosters.Remove(poster);
        }

        franchise.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Franchise> FindFranchise(CineasteList list, Id<Franchise> id, CancellationToken token)
    {
        var franchise = list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw new FranchiseNotFoundException(id);

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

    private async Task<CineasteList> FindList(Id<CineasteList> listId, CancellationToken token)
    {
        var list = await this.dbContext.Lists.SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

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
            ? list.MovieKinds.FirstOrDefault(kind => kind.Id == movieKindId)
                ?? throw new MovieKindNotFoundException(movieKindId)
            : list.MovieKinds.First();
    }

    private SeriesKind ResolveSeriesKind(FranchiseRequest request, CineasteList list)
    {
        var seriesKindId = Id.For<SeriesKind>(request.KindId);
        return request.KindSource == FranchiseKindSource.Series
            ? list.SeriesKinds.FirstOrDefault(kind => kind.Id == seriesKindId)
                ?? throw new SeriesKindNotFoundException(seriesKindId)
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
            throw new FranchiseItemsNotFoundException(missingMovieIds, missingSeriesIds, missingFranchiseIds);
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

    private Task<PosterHash> SetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetFranchisePoster(
            listId, franchiseId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetFranchisePoster(
        Id<CineasteList> listId,
        Id<Franchise> franchiseId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetFranchisePoster(franchiseId, listId);

        var list = await this.FindList(listId, token);
        var franchise = await this.FindFranchise(list, franchiseId, token);

        var content = await getContent();

        if (await this.dbContext.FranchisePosters.FirstOrDefaultAsync(poster => poster.Franchise == franchise, token)
            is { } existingPoster)
        {
            this.dbContext.FranchisePosters.Remove(existingPoster);
        }

        var poster = new FranchisePoster(Id.Create<FranchisePoster>(), franchise, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        franchise.PosterHash = hash;

        this.dbContext.FranchisePosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting franchise {FranchiseId} in list {ListId}")]
    private partial void LogGetFranchise(Id<Franchise> franchiseId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Adding franchise to list {ListId}")]
    private partial void LogAddFranchise(Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating franchise {FranchiseId} in list {ListId}")]
    private partial void LogUpdateFranchise(Id<Franchise> franchiseId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing franchise {FranchiseId} from list {ListId}")]
    private partial void LogRemoveFranchise(Id<Franchise> franchiseId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting the poster for franchise {FranchiseId} in list {ListId}")]
    private partial void LogGetFranchisePoster(Id<Franchise> franchiseId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Setting the poster for franchise {FranchiseId} in list {ListId}")]
    private partial void LogSetFranchisePoster(Id<Franchise> franchiseId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Removing the poster for franchise {FranchiseId} in list {ListId}")]
    private partial void LogRemoveFranchisePoster(Id<Franchise> franchiseId, Id<CineasteList> listId);
}
