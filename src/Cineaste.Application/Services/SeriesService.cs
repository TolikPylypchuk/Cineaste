namespace Cineaste.Application.Services;

public sealed partial class SeriesService(
    CineasteDbContext dbContext,
    IPosterProvider posterProvider,
    IPosterUrlProvider posterUrlProvider,
    ILogger<SeriesService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IPosterProvider posterProvider = posterProvider;
    private readonly IPosterUrlProvider posterUrlProvider = posterUrlProvider;

    public async Task<SeriesModel> GetSeries(Id<CineasteList> listId, Id<Series> id, CancellationToken token)
    {
        this.LogGetSeries(id, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, id, token);

        return series.ToSeriesModel(this.posterUrlProvider);
    }

    public async Task<SeriesModel> AddSeries(
        Id<CineasteList> listId,
        Validated<SeriesRequest> request,
        CancellationToken token)
    {
        this.LogAddSeries(listId);

        var list = await this.FindList(listId, token);
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

        return series.ToSeriesModel(this.posterUrlProvider);
    }

    public async Task<SeriesModel> UpdateSeries(
        Id<CineasteList> listId,
        Id<Series> id,
        Validated<SeriesRequest> request,
        CancellationToken token)
    {
        this.LogUpdateSeries(id, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, id, token);

        var kind = this.FindKind(list, Id.For<SeriesKind>(request.Value.KindId));

        series.Update(request, kind);
        series.ListItem?.SetProperties(series);

        list.SortItems();

        await dbContext.SaveChangesAsync(token);

        return series.ToSeriesModel(this.posterUrlProvider);
    }

    public async Task RemoveSeries(Id<CineasteList> listId, Id<Series> id, CancellationToken token)
    {
        this.LogRemoveSeries(id, listId);

        var list = await this.FindList(listId, token);
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

    public async Task<BinaryContent> GetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        CancellationToken token)
    {
        this.LogGetSeriesPoster(seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);

        var poster = await this.dbContext.SeriesPosters
            .Where(poster => poster.Series == series)
            .FirstOrDefaultAsync(token)
            ?? throw new SeriesPosterNotFoundException(seriesId);

        return poster.ToPosterModel();
    }

    public async Task<PosterHash> SetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        StreamableContent content,
        CancellationToken token) =>
        await this.SetSeriesPoster(listId, seriesId, () => Task.FromResult(content), token);

    public async Task<PosterHash> SetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        await this.SetSeriesPoster(listId, seriesId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task<PosterHash> SetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        await this.SetSeriesPoster(listId, seriesId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveSeriesPoster(Id<CineasteList> listId, Id<Series> seriesId, CancellationToken token)
    {
        this.LogRemoveSeriesPoster(seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);

        var poster = await this.dbContext.SeriesPosters
            .Where(poster => poster.Series == series)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.SeriesPosters.Remove(poster);
        }

        series.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<BinaryContent> GetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        CancellationToken token)
    {
        this.LogGetSeasonPoster(periodId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var period = this.FindPeriod(series, periodId);

        var poster = await this.dbContext.SeasonPosters
            .Where(poster => poster.Period == period)
            .FirstOrDefaultAsync(token)
            ?? throw new SeasonPosterNotFoundException(periodId);

        return poster.ToPosterModel();
    }

    public async Task<PosterHash> SetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        StreamableContent content,
        CancellationToken token) =>
        await this.SetSeasonPoster(listId, seriesId, periodId, () => Task.FromResult(content), token);

    public async Task<PosterHash> SetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        await this.SetSeasonPoster(
            listId, seriesId, periodId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task<PosterHash> SetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        await this.SetSeasonPoster(
            listId, seriesId, periodId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        CancellationToken token)
    {
        this.LogRemoveSeasonPoster(periodId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var period = this.FindPeriod(series, periodId);

        var poster = await this.dbContext.SeasonPosters
            .Where(poster => poster.Period == period)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.SeasonPosters.Remove(poster);
        }

        period.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<BinaryContent> GetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        CancellationToken token)
    {
        this.LogGetSpecialEpisodePoster(episodeId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var episode = this.FindSpecialEpisode(series, episodeId);

        var poster = await this.dbContext.SpecialEpisodePosters
            .Where(poster => poster.SpecialEpisode == episode)
            .FirstOrDefaultAsync(token)
            ?? throw new SpecialEpisodePosterNotFoundException(episodeId);

        return poster.ToPosterModel();
    }

    public async Task<PosterHash> SetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        StreamableContent content,
        CancellationToken token) =>
        await this.SetSpecialEpisodePoster(listId, seriesId, episodeId, () => Task.FromResult(content), token);

    public async Task<PosterHash> SetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        await this.SetSpecialEpisodePoster(
            listId, seriesId, episodeId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task<PosterHash> SetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        await this.SetSpecialEpisodePoster(
            listId, seriesId, episodeId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        CancellationToken token)
    {
        this.LogRemoveSpecialEpisodePoster(episodeId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var episode = this.FindSpecialEpisode(series, episodeId);

        var poster = await this.dbContext.SpecialEpisodePosters
            .Where(poster => poster.SpecialEpisode == episode)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.SpecialEpisodePosters.Remove(poster);
        }

        episode.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    private Period FindPeriod(Series series, Id<Period> periodId) =>
        series.Seasons
            .SelectMany(season => season.Periods)
            .FirstOrDefault(period => period.Id == periodId)
            ?? throw new PeriodNotFoundException(periodId);

    private SpecialEpisode FindSpecialEpisode(Series series, Id<SpecialEpisode> episodeId) =>
        series.SpecialEpisodes
            .FirstOrDefault(episode => episode.Id == episodeId)
            ?? throw new SpecialEpisodeNotFoundException(episodeId);

    private async Task<Series> FindSeries(CineasteList list, Id<Series> id, CancellationToken token)
    {
        var series = list.Items
            .Select(item => item.Series)
            .WhereNotNull()
            .FirstOrDefault(series => series.Id == id)
            ?? throw new SeriesNotFoundException(id);

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
            ?? throw new SeriesKindNotFoundException(id);

    private Franchise FindFranchise(CineasteList list, Id<Franchise> id) =>
        list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw new FranchiseNotFoundException(id);

    private Task<PosterHash> SetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetSeriesPoster(listId, seriesId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetSeriesPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetSeriesPoster(seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);

        var content = await getContent();

        if (await this.dbContext.SeriesPosters.FirstOrDefaultAsync(poster => poster.Series == series, token)
            is { } existingPoster)
        {
            this.dbContext.SeriesPosters.Remove(existingPoster);
        }

        var poster = new SeriesPoster(Id.Create<SeriesPoster>(), series, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        series.PosterHash = hash;

        this.dbContext.SeriesPosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    private Task<PosterHash> SetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetSeasonPoster(
            listId, seriesId, periodId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetSeasonPoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<Period> periodId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetSeasonPoster(periodId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var period = this.FindPeriod(series, periodId);

        var content = await getContent();

        if (await this.dbContext.SeasonPosters.FirstOrDefaultAsync(poster => poster.Period == period, token)
            is { } existingPoster)
        {
            this.dbContext.SeasonPosters.Remove(existingPoster);
        }

        var poster = new SeasonPoster(Id.Create<SeasonPoster>(), period, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        period.PosterHash = hash;

        this.dbContext.SeasonPosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    private Task<PosterHash> SetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetSpecialEpisodePoster(
            listId, seriesId, episodeId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetSpecialEpisodePoster(
        Id<CineasteList> listId,
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetSpecialEpisodePoster(episodeId, seriesId, listId);

        var list = await this.FindList(listId, token);
        var series = await this.FindSeries(list, seriesId, token);
        var episode = this.FindSpecialEpisode(series, episodeId);

        var content = await getContent();

        if (await this.dbContext.SpecialEpisodePosters
                .FirstOrDefaultAsync(poster => poster.SpecialEpisode == episode, token) is { } existingPoster)
        {
            this.dbContext.SpecialEpisodePosters.Remove(existingPoster);
        }

        var poster = new SpecialEpisodePoster(Id.Create<SpecialEpisodePoster>(), episode, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        episode.PosterHash = hash;

        this.dbContext.SpecialEpisodePosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting series {SeriesId} in list {ListId}")]
    private partial void LogGetSeries(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Adding a series to list {ListId}")]
    private partial void LogAddSeries(Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating series {SeriesId} in list {ListId}")]
    private partial void LogUpdateSeries(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing series {SeriesId} from list {ListId}")]
    private partial void LogRemoveSeries(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting the poster for series {SeriesId} in list {ListId}")]
    private partial void LogGetSeriesPoster(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Setting the poster for series {SeriesId} in list {ListId}")]
    private partial void LogSetSeriesPoster(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing the poster for series {SeriesId} in list {ListId}")]
    private partial void LogRemoveSeriesPoster(Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Getting the poster for season period {PeriodId} of series {SeriesId} in list {ListId}")]
    private partial void LogGetSeasonPoster(Id<Period> periodId, Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Setting the poster for season period {PeriodId} of series {SeriesId} in list {ListId}")]
    private partial void LogSetSeasonPoster(Id<Period> periodId, Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Removing the poster for season period {PeriodId} of series {SeriesId} in list {ListId}")]
    private partial void LogRemoveSeasonPoster(Id<Period> periodId, Id<Series> seriesId, Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Getting the poster for special episode {EpisodeId} of series {SeriesId} in list {ListId}")]
    private partial void LogGetSpecialEpisodePoster(
        Id<SpecialEpisode> episodeId,
        Id<Series> seriesId,
        Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Setting the poster for special episode {EpisodeId} of series {SeriesId} in list {ListId}")]
    private partial void LogSetSpecialEpisodePoster(
        Id<SpecialEpisode> episodeId,
        Id<Series> seriesId,
        Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Removing the poster for special episode {EpisodeId} of series {SeriesId} in list {ListId}")]
    private partial void LogRemoveSpecialEpisodePoster(
        Id<SpecialEpisode> episodeId,
        Id<Series> seriesId,
        Id<CineasteList> listId);
}
