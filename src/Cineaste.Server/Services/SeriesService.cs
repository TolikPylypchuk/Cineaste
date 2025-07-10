namespace Cineaste.Server.Services;

public sealed class SeriesService(CineasteDbContext dbContext, ILogger<SeriesService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<SeriesService> logger = logger;

    public async Task<SeriesModel> GetSeries(Id<Series> id)
    {
        this.logger.LogDebug("Getting the series with ID: {Id}", id.Value);

        var series = await this.FindSeries(id);
        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> CreateSeries(Validated<SeriesRequest> request)
    {
        this.logger.LogDebug("Creating a new series");

        var list = await this.FindList(request.Value.ListId);
        var kind = await this.FindKind(request.Value.KindId, list);

        var series = request.ToSeries(Id.Create<Series>(), kind);

        list.AddSeries(series);
        dbContext.Series.Add(series);

        await dbContext.SaveChangesAsync();

        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> UpdateSeries(Id<Series> id, Validated<SeriesRequest> request)
    {
        this.logger.LogDebug("Updating the series with ID: {Id}", id.Value);

        var series = await this.FindSeries(id);
        var list = await this.FindList(request.Value.ListId);

        if (!list.Series.Contains(series))
        {
            throw this.SeriesDoesNotBelongToList(id, list.Id);
        }

        var kind = await this.FindKind(request.Value.KindId, list);

        series.Update(request, kind);

        await dbContext.SaveChangesAsync();

        return series.ToSeriesModel();
    }

    public async Task DeleteSeries(Id<Series> id)
    {
        this.logger.LogDebug("Deleting the series with ID: {Id}", id.Value);

        var series = await this.dbContext.Series.FindAsync(id) ?? throw this.NotFound(id);

        this.dbContext.Series.Remove(series);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task<Series> FindSeries(Id<Series> id)
    {
        var series = await this.dbContext.Series
            .Include(series => series.Titles)
            .Include(series => series.Kind)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.Titles)
            .Include(series => series.Seasons)
                .ThenInclude(season => season.Periods)
            .Include(series => series.SpecialEpisodes)
                .ThenInclude(episode => episode.Titles)
            .Include(series => series.Tags)
            .AsSplitQuery()
            .SingleOrDefaultAsync(series => series.Id == id);

        return series is not null ? series : throw this.NotFound(id);
    }

    private async Task<CineasteList> FindList(Guid id)
    {
        var listId = Id.For<CineasteList>(id);

        var list = await this.dbContext.Lists
            .Include(list => list.Series)
                .ThenInclude(series => series.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.Kind)
            .Include(list => list.Series)
                .ThenInclude(series => series.Seasons)
                    .ThenInclude(season => season.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(list => list.Series)
                .ThenInclude(series => series.SpecialEpisodes)
                    .ThenInclude(episode => episode.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.Tags)
            .Include(list => list.SeriesKinds)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId);

        return list is not null
            ? list
            : throw this.NotFound(listId);
    }

    private async Task<SeriesKind> FindKind(Guid id, CineasteList list)
    {
        var kindId = Id.For<SeriesKind>(id);
        var kind = await this.dbContext.SeriesKinds.FindAsync(kindId);

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
