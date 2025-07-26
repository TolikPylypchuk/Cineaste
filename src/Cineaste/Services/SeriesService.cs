namespace Cineaste.Services;

public sealed class SeriesService(CineasteDbContext dbContext, ILogger<SeriesService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<SeriesService> logger = logger;

    public async Task<SeriesModel> GetSeries(Id<Series> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the series with ID: {Id}", id.Value);

        var series = await this.FindSeries(id, token);
        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> AddSeries(Validated<SeriesRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Creating a new series");

        var list = await this.FindList(request.Value.ListId, token);
        var kind = await this.FindKind(request.Value.KindId, list, token);

        var series = request.ToSeries(Id.Create<Series>(), kind);

        list.AddSeries(series);
        dbContext.Series.Add(series);

        list.SortItems();

        await dbContext.SaveChangesAsync(token);

        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> UpdateSeries(
        Id<Series> id,
        Validated<SeriesRequest> request,
        CancellationToken token)
    {
        this.logger.LogDebug("Updating the series with ID: {Id}", id.Value);

        var series = await this.FindSeries(id, token);
        var list = await this.FindList(request.Value.ListId, token);

        if (!list.ContainsSeries(series))
        {
            throw this.SeriesDoesNotBelongToList(id, list.Id);
        }

        var kind = await this.FindKind(request.Value.KindId, list, token);

        series.Update(request, kind);
        series.ListItem?.SetProperties(series);

        list.SortItems();

        await dbContext.SaveChangesAsync(token);

        return series.ToSeriesModel();
    }

    public async Task RemoveSeries(Id<Series> id, CancellationToken token)
    {
        this.logger.LogDebug("Deleting the series with ID: {Id}", id.Value);

        var series = await this.dbContext.Series
            .Where(series => series.Id == id)
            .Include(series => series.ListItem)
                .ThenInclude(item => item!.List)
            .SingleOrDefaultAsync(token)
            ?? throw this.NotFound(id);

        var listItem = series.ListItem!;

        var list = await this.FindList(listItem.List.Id.Value, token);
        list.RemoveSeries(series);
        list.SortItems();

        dbContext.ListItems.Remove(listItem);

        this.dbContext.Series.Remove(series);
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Series> FindSeries(Id<Series> id, CancellationToken token)
    {
        var series = await this.dbContext.Series
            .Include(series => series.AllTitles)
            .Include(series => series.Kind)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.AllTitles)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.Periods)
            .Include(series => series.SpecialEpisodes)
                .ThenInclude(episode => episode.AllTitles)
            .Include(series => series.Tags)
            .Include(series => series.FranchiseItem)
                .ThenInclude(item => item!.ParentFranchise)
                    .ThenInclude(franchise => franchise.Children)
            .AsSplitQuery()
            .SingleOrDefaultAsync(series => series.Id == id, token);

        return series is not null ? series : throw this.NotFound(id);
    }

    private async Task<CineasteList> FindList(Guid id, CancellationToken token)
    {
        var listId = Id.For<CineasteList>(id);

        return await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.AllTitles)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.Kind)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.Seasons)
                        .ThenInclude(season => season.AllTitles)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.Seasons)
                        .ThenInclude(season => season.Periods)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.SpecialEpisodes)
                        .ThenInclude(episode => episode.AllTitles)
            .Include(list => list.Items)
                .ThenInclude(item => item.Series)
                    .ThenInclude(series => series!.Tags)
            .Include(list => list.SeriesKinds)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId, token)
             ?? throw this.NotFound(listId);
    }

    private async Task<SeriesKind> FindKind(Guid id, CineasteList list, CancellationToken token)
    {
        var kindId = Id.For<SeriesKind>(id);
        var kind = await this.dbContext.SeriesKinds.FindAsync([kindId], token);

        if (kind is null)
        {
            throw this.NotFound(kindId);
        } else if (!list.SeriesKinds.Contains(kind))
        {
            throw this.KindDoesNotBelongToList(kindId, list.Id);
        }

        return kind;
    }

    private CineasteException NotFound(Id<Series> id) =>
        new NotFoundException(Resources.Series, $"Could not find a series with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<CineasteList> id) =>
        new NotFoundException(Resources.List, $"Could not find a list with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<SeriesKind> id) =>
        new NotFoundException(Resources.SeriesKind, $"Could not find a series kind with ID {id.Value}")
            .WithProperty(id);

    private CineasteException SeriesDoesNotBelongToList(Id<Series> seriesId, Id<CineasteList> listId) =>
        new InvalidInputException(
            $"{Resources.Series}.WrongList",
            $"Series with ID {seriesId.Value} does not belong to list with ID {listId}")
            .WithProperty(seriesId)
            .WithProperty(listId);

    private CineasteException KindDoesNotBelongToList(Id<SeriesKind> kindId, Id<CineasteList> listId) =>
        new InvalidInputException(
            $"{Resources.SeriesKind}.WrongList",
            $"Movie kind with ID {kindId.Value} does not belong to list with ID {listId}")
            .WithProperty(kindId)
            .WithProperty(listId);
}
