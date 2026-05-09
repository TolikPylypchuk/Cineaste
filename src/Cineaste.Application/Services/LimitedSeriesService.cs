namespace Cineaste.Application.Services;

public sealed partial class LimitedSeriesService(
    CineasteDbContext dbContext,
    IPosterProvider posterProvider,
    IPosterUrlProvider posterUrlProvider,
    ILogger<LimitedSeriesService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IPosterProvider posterProvider = posterProvider;
    private readonly IPosterUrlProvider posterUrlProvider = posterUrlProvider;

    public async Task<LimitedSeriesModel> GetLimitedSeries(
        Id<CineasteList> listId, Id<LimitedSeries> id, CancellationToken token)
    {
        this.LogGetLimitedSeries(id, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, id, token);

        return limitedSeries.ToLimitedSeriesModel(this.posterUrlProvider);
    }

    public async Task<LimitedSeriesModel> AddLimitedSeries(
        Id<CineasteList> listId,
        Validated<LimitedSeriesRequest> request,
        CancellationToken token)
    {
        this.LogAddLimitedSeries(listId);

        var list = await this.FindList(listId, token);
        var kind = this.FindKind(list, Id.For<SeriesKind>(request.Value.KindId));

        var limitedSeries = request.ToLimitedSeries(Id.Create<LimitedSeries>(), kind);

        if (request.Value.ParentFranchiseId is Guid franchiseId)
        {
            var franchise = this.FindFranchise(list, Id.For<Franchise>(franchiseId));
            var franchiseItem = franchise.AttachLimitedSeries(limitedSeries, true);
            this.dbContext.FranchiseItems.Add(franchiseItem);
        }

        list.AddLimitedSeries(limitedSeries);
        list.SortItems();

        this.dbContext.LimitedSeries.Add(limitedSeries);
        await this.dbContext.SaveChangesAsync(token);

        return limitedSeries.ToLimitedSeriesModel(this.posterUrlProvider);
    }

    public async Task<LimitedSeriesModel> UpdateLimitedSeries(
        Id<CineasteList> listId,
        Id<LimitedSeries> id,
        Validated<LimitedSeriesRequest> request,
        CancellationToken token)
    {
        this.LogUpdateLimitedSeries(id, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, id, token);

        var kind = this.FindKind(list, Id.For<SeriesKind>(request.Value.KindId));

        limitedSeries.Update(request, kind);
        limitedSeries.ListItem?.SetProperties(limitedSeries);

        list.SortItems();

        await this.dbContext.SaveChangesAsync(token);

        return limitedSeries.ToLimitedSeriesModel(this.posterUrlProvider);
    }

    public async Task RemoveLimitedSeries(Id<CineasteList> listId, Id<LimitedSeries> id, CancellationToken token)
    {
        this.LogRemoveLimitedSeries(id, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, id, token);

        if (limitedSeries.FranchiseItem is { } item)
        {
            item.ParentFranchise.DetachLimitedSeries(limitedSeries);
        }

        list.RemoveLimitedSeries(limitedSeries);
        list.SortItems();

        this.dbContext.ListItems.Remove(limitedSeries.ListItem!);
        this.dbContext.LimitedSeries.Remove(limitedSeries);

        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<BinaryContent> GetLimitedSeriesPoster(
        Id<CineasteList> listId, Id<LimitedSeries> limitedSeriesId, CancellationToken token)
    {
        this.LogGetLimitedSeriesPoster(limitedSeriesId, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, limitedSeriesId, token);

        var poster = await this.dbContext.LimitedSeriesPosters
            .Where(poster => poster.LimitedSeries == limitedSeries)
            .FirstOrDefaultAsync(token)
            ?? throw new LimitedSeriesPosterNotFoundException(limitedSeriesId);

        return poster.ToPosterModel();
    }

    public Task<PosterHash> SetLimitedSeriesPoster(
        Id<CineasteList> listId,
        Id<LimitedSeries> limitedSeriesId,
        StreamableContent content,
        CancellationToken token) =>
        this.SetLimitedSeriesPoster(listId, limitedSeriesId, () => Task.FromResult(content), token);

    public Task<PosterHash> SetLimitedSeriesPoster(
        Id<CineasteList> listId,
        Id<LimitedSeries> limitedSeriesId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        this.SetLimitedSeriesPoster(
            listId, limitedSeriesId, () => this.posterProvider.FetchPoster(request, token), token);

    public Task<PosterHash> SetLimitedSeriesPoster(
        Id<CineasteList> listId,
        Id<LimitedSeries> limitedSeriesId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        this.SetLimitedSeriesPoster(
            listId, limitedSeriesId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveLimitedSeriesPoster(
        Id<CineasteList> listId, Id<LimitedSeries> limitedSeriesId, CancellationToken token)
    {
        this.LogRemoveLimitedSeriesPoster(limitedSeriesId, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, limitedSeriesId, token);

        var poster = await this.dbContext.LimitedSeriesPosters
            .Where(poster => poster.LimitedSeries == limitedSeries)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.LimitedSeriesPosters.Remove(poster);
        }

        limitedSeries.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<LimitedSeries> FindLimitedSeries(
        CineasteList list, Id<LimitedSeries> id, CancellationToken token)
    {
        var limitedSeries = list.Items
            .Select(item => item.LimitedSeries)
            .WhereNotNull()
            .FirstOrDefault(limitedSeries => limitedSeries.Id == id)
            ?? throw new LimitedSeriesNotFoundException(id);

        await this.dbContext.Entry(limitedSeries)
            .Reference(m => m.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (limitedSeries.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(limitedSeries.FranchiseItem.ParentFranchise, token);
        }

        return limitedSeries;
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
                    .ThenInclude(season => season.Parts)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.LimitedSeries)
                .ThenInclude(limitedSeries => limitedSeries!.AllTitles)
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
                    .ThenInclude(item => item!.ParentFranchise)
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Parts)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.LimitedSeries)
                .ThenInclude(limitedSeries => limitedSeries!.AllTitles)
            .Include(item => item.LimitedSeries)
                .ThenInclude(limitedSeries => limitedSeries!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .AsSplitQuery()
            .LoadAsync(token);

        return list;
    }

    private SeriesKind FindKind(CineasteList list, Id<SeriesKind> id) =>
        list.SeriesKinds
            .FirstOrDefault(kind => kind.Id == id)
            ?? throw new SeriesKindNotFoundException(id);

    private Franchise FindFranchise(CineasteList list, Id<Franchise> id) =>
        list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw new FranchiseNotFoundException(id);

    private Task<PosterHash> SetLimitedSeriesPoster(
        Id<CineasteList> listId,
        Id<LimitedSeries> limitedSeriesId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetLimitedSeriesPoster(
            listId, limitedSeriesId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetLimitedSeriesPoster(
        Id<CineasteList> listId,
        Id<LimitedSeries> limitedSeriesId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetLimitedSeriesPoster(limitedSeriesId, listId);

        var list = await this.FindList(listId, token);
        var limitedSeries = await this.FindLimitedSeries(list, limitedSeriesId, token);

        var content = await getContent();

        if (await this.dbContext.LimitedSeriesPosters
            .FirstOrDefaultAsync(poster => poster.LimitedSeries == limitedSeries, token) is { } existingPoster)
        {
            this.dbContext.LimitedSeriesPosters.Remove(existingPoster);
        }

        var poster = new LimitedSeriesPoster(
            Id.Create<LimitedSeriesPoster>(), limitedSeries, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        limitedSeries.PosterHash = hash;

        this.dbContext.LimitedSeriesPosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting limited series {LimitedSeriesId} in list {ListId}")]
    private partial void LogGetLimitedSeries(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Adding a limited series to list {ListId}")]
    private partial void LogAddLimitedSeries(Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating limited series {LimitedSeriesId} in list {ListId}")]
    private partial void LogUpdateLimitedSeries(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing limited series {LimitedSeriesId} from list {ListId}")]
    private partial void LogRemoveLimitedSeries(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug, Message = "Getting the poster for limited series {LimitedSeriesId} in list {ListId}")]
    private partial void LogGetLimitedSeriesPoster(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug, Message = "Setting the poster for limited series {LimitedSeriesId} in list {ListId}")]
    private partial void LogSetLimitedSeriesPoster(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug, Message = "Removing the poster for limited series {LimitedSeriesId} in list {ListId}")]
    private partial void LogRemoveLimitedSeriesPoster(Id<LimitedSeries> limitedSeriesId, Id<CineasteList> listId);
}
