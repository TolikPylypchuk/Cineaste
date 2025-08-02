using Cineaste.Core.Domain;

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

        if (request.Value.ParentFranchiseId is Guid franchiseId)
        {
            var franchise = this.FindFranchise(list, Id.For<Franchise>(franchiseId));
            var franchiseItem = franchise.AttachSeries(series, true);
            this.dbContext.FranchiseItems.Add(franchiseItem);
        }

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

    public async Task<PosterModel> GetSeriesPoster(Id<Series> seriesId, CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);

        var poster = await this.dbContext.SeriesPosters
            .Where(poster => poster.Series == series)
            .FirstOrDefaultAsync(token)
            ?? throw this.PosterNotFound(seriesId);

        return poster.ToPosterModel();
    }

    public async Task SetSeriesPoster(Id<Series> seriesId, PosterRequest request, CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);

        var data = new byte[request.ContentLength];
        await request.Data.ReadExactlyAsync(data, 0, data.Length, token);

        var poster = new SeriesPoster(Id.Create<SeriesPoster>(), series, data, request.ContentType);

        this.dbContext.SeriesPosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<PosterModel> GetSeasonPoster(Id<Series> seriesId, Id<Period> periodId, CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);
        var period = this.FindPeriod(series, periodId);

        var poster = await this.dbContext.SeasonPosters
            .Where(poster => poster.Period == period)
            .FirstOrDefaultAsync(token)
            ?? throw this.PosterNotFound(periodId);

        return poster.ToPosterModel();
    }

    public async Task SetSeasonPoster(
        Id<Series> seriesId,
        Id<Period> periodId,
        PosterRequest request,
        CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);
        var period = this.FindPeriod(series, periodId);

        var data = new byte[request.ContentLength];
        await request.Data.ReadExactlyAsync(data, 0, data.Length, token);

        var poster = new SeasonPoster(Id.Create<SeasonPoster>(), period, data, request.ContentType);

        this.dbContext.SeasonPosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<PosterModel> GetSpecialEpisodePoster(
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);
        var episode = this.FindSpecialEpisode(series, episodeId);

        var poster = await this.dbContext.SpecialEpisodePosters
            .Where(poster => poster.SpecialEpisode == episode)
            .FirstOrDefaultAsync(token)
            ?? throw this.PosterNotFound(episodeId);

        return poster.ToPosterModel();
    }

    public async Task SetSpecialEpisodePoster(
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        PosterRequest request,
        CancellationToken token)
    {
        var list = await this.FindList(token);
        var series = await this.FindSeries(list, seriesId, token);
        var episode = this.FindSpecialEpisode(series, episodeId);

        var data = new byte[request.ContentLength];
        await request.Data.ReadExactlyAsync(data, 0, data.Length, token);

        var poster = new SpecialEpisodePoster(Id.Create<SpecialEpisodePoster>(), episode, data, request.ContentType);

        this.dbContext.SpecialEpisodePosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);
    }

    private Period FindPeriod(Series series, Id<Period> periodId) =>
        series.Seasons
            .SelectMany(season => season.Periods)
            .FirstOrDefault(period => period.Id == periodId)
            ?? throw this.NotFound(periodId);

    private SpecialEpisode FindSpecialEpisode(Series series, Id<SpecialEpisode> episodeId) =>
        series.SpecialEpisodes
            .FirstOrDefault(episode => episode.Id == episodeId)
            ?? throw this.NotFound(episodeId);

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
                    .ThenInclude(item => item!.ParentFranchise)
            .AsSplitQuery()
            .LoadAsync(token);

        return list;
    }

    private SeriesKind FindKind(CineasteList list, Id<SeriesKind> id) =>
        list.SeriesKinds
            .FirstOrDefault(kind => kind.Id == id)
            ?? throw this.NotFound(id);

    private Franchise FindFranchise(CineasteList list, Id<Franchise> id) =>
        list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw this.NotFound(id);

    private NotFoundException ListNotFound() =>
        new(Resources.List, "Could not find the list");

    private CineasteException NotFound(Id<Series> id) =>
        new NotFoundException(Resources.Series, $"Could not find a series with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<Period> id) =>
        new NotFoundException(Resources.Period, $"Could not find a period with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<SpecialEpisode> id) =>
        new NotFoundException(Resources.SpecialEpisode, $"Could not find a special episode with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<SeriesKind> id) =>
        new NotFoundException(Resources.SeriesKind, $"Could not find a series kind with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with ID {id.Value}")
            .WithProperty(id);

    private CineasteException PosterNotFound(Id<Series> seriesId) =>
        new NotFoundException(Resources.Poster, $"Could not find a poster for series with ID {seriesId.Value}")
            .WithProperty(seriesId);

    private CineasteException PosterNotFound(Id<Period> periodId) =>
        new NotFoundException(Resources.Poster, $"Could not find a poster for period with ID {periodId.Value}")
            .WithProperty(periodId);

    private CineasteException PosterNotFound(Id<SpecialEpisode> episodeId) =>
        new NotFoundException(Resources.Poster, $"Could not find a poster for episode with ID {episodeId.Value}")
            .WithProperty(episodeId);
}
