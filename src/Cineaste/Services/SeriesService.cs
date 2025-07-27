namespace Cineaste.Services;

public sealed class SeriesService(CineasteDbContext dbContext, ILogger<SeriesService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<SeriesService> logger = logger;

    public async Task<SeriesModel> GetSeries(Id<Series> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the series with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var series = await this.FindSeries(list, id, token);

        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> AddSeries(Validated<SeriesRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Adding a new series");

        var list = await this.FindList(token);
        var kind = this.FindKind(list, Id.For<SeriesKind>(request.Value.KindId));

        var series = request.ToSeries(Id.Create<Series>(), kind);

        list.AddSeries(series);
        list.SortItems();

        dbContext.Series.Add(series);
        await dbContext.SaveChangesAsync(token);

        return series.ToSeriesModel();
    }

    public async Task<SeriesModel> UpdateSeries(
        Id<Series> id,
        Validated<SeriesRequest> request,
        CancellationToken token)
    {
        this.logger.LogDebug("Updating the series with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var series = await this.FindSeries(list, id, token);

        var kind = this.FindKind(list, Id.For<SeriesKind>(request.Value.KindId));

        series.Update(request, kind);
        series.ListItem?.SetProperties(series);

        list.SortItems();

        await dbContext.SaveChangesAsync(token);

        return series.ToSeriesModel();
    }

    public async Task RemoveSeries(Id<Series> id, CancellationToken token)
    {
        this.logger.LogDebug("Removing the series with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var series = await this.FindSeries(list, id, token);

        if (series.FranchiseItem is { } item)
        {
            item.ParentFranchise.DetachSeries(series);
        }

        list.RemoveSeries(series);
        list.SortItems();

        this.dbContext.ListItems.Remove(series.ListItem!);
        this.dbContext.Series.Remove(series);
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Series> FindSeries(CineasteList list, Id<Series> id, CancellationToken token)
    {
        var series = list.Items
            .Select(item => item.Series)
            .WhereNotNull()
            .FirstOrDefault(series => series.Id == id)
            ?? throw this.NotFound(id);

        await this.dbContext.Entry(series)
            .Collection(s => s.Seasons)
            .Query()
            .Include(s => s.AllTitles)
            .Include(s => s.Periods)
            .AsSplitQuery()
            .LoadAsync(token);

        await this.dbContext.Entry(series)
            .Collection(s => s.SpecialEpisodes)
            .Query()
            .Include(e => e.AllTitles)
            .LoadAsync(token);

        await this.dbContext.Entry(series)
            .Reference(s => s.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (series.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(series.FranchiseItem.ParentFranchise, token);
        }

        return series;
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
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .LoadAsync(token);

        return list;
    }

    private SeriesKind FindKind(CineasteList list, Id<SeriesKind> id) =>
        list.SeriesKinds
            .FirstOrDefault(kind => kind.Id == id)
            ?? throw this.NotFound(id);

    private NotFoundException ListNotFound() =>
        new(Resources.List, "Could not find the list");

    private CineasteException NotFound(Id<Series> id) =>
        new NotFoundException(Resources.Series, $"Could not find a series with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<SeriesKind> id) =>
        new NotFoundException(Resources.SeriesKind, $"Could not find a series kind with ID {id.Value}")
            .WithProperty(id);
}
