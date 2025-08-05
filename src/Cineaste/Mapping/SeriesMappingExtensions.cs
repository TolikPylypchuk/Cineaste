namespace Cineaste.Mapping;

public static class SeriesMappingExtensions
{
    public static SeriesModel ToSeriesModel(this Series series) =>
        new(
            series.Id.Value,
            series.AllTitles.ToTitleModels(isOriginal: false),
            series.AllTitles.ToTitleModels(isOriginal: true),
            series.WatchStatus,
            series.ReleaseStatus,
            series.Kind.ToListKindModel(),
            [.. series.Seasons.Select(season => season.ToSeasonModel(series))],
            [.. series.SpecialEpisodes.Select(episode => episode.ToSpecialEpisodeModel(series))],
            series.ImdbId,
            series.RottenTomatoesId,
            series.GetActiveColor()?.HexValue ?? String.Empty,
            series.ListItem?.SequenceNumber ?? 0,
            series.GetPosterUrl(),
            series.FranchiseItem.ToFranchiseItemInfoModel());

    public static Series ToSeries(this Validated<SeriesRequest> request, Id<Series> id, SeriesKind kind) =>
        new(
            id,
            request.Value.ToTitles(),
            request.Value.Seasons.Select(ToSeason),
            request.Value.SpecialEpisodes.Select(ToSpecialEpisode),
            request.Value.WatchStatus,
            request.Value.ReleaseStatus,
            kind)
        {
            ImdbId = request.Value.ImdbId,
            RottenTomatoesId = request.Value.RottenTomatoesId
        };

    public static void Update(this Series series, Validated<SeriesRequest> request, SeriesKind kind)
    {
        series.ReplaceTitles(
            request.Value.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: false);

        series.ReplaceTitles(
            request.Value.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: true);

        series.WatchStatus = request.Value.WatchStatus;
        series.ReleaseStatus = request.Value.ReleaseStatus;
        series.Kind = kind;
        series.ImdbId = request.Value.ImdbId;
        series.RottenTomatoesId = request.Value.RottenTomatoesId;

        series.UpdateSeasons(request.Value.Seasons);
        series.UpdateSpecialEpisodes(request.Value.SpecialEpisodes);

        series.ListItem?.SetProperties(series);
    }

    private static SeasonModel ToSeasonModel(this Season season, Series series) =>
        new(
            season.Id.Value,
            season.AllTitles.ToTitleModels(isOriginal: false),
            season.AllTitles.ToTitleModels(isOriginal: true),
            season.SequenceNumber,
            season.WatchStatus,
            season.ReleaseStatus,
            season.Channel,
            [.. season.Periods
                .Select(period => period.ToPeriodModel(series))
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .ThenBy(period => period.EndYear)
                .ThenBy(period => period.EndMonth)]);

    private static PeriodModel ToPeriodModel(this Period period, Series series) =>
        new(
            period.Id.Value,
            period.StartMonth,
            period.StartYear,
            period.EndMonth,
            period.EndYear,
            period.EpisodeCount,
            period.IsSingleDayRelease,
            period.RottenTomatoesId,
            series.GetPosterUrl(period));

    private static SpecialEpisodeModel ToSpecialEpisodeModel(this SpecialEpisode episode, Series series) =>
        new(
            episode.Id.Value,
            episode.AllTitles.ToTitleModels(isOriginal: false),
            episode.AllTitles.ToTitleModels(isOriginal: true),
            episode.SequenceNumber,
            episode.IsWatched,
            episode.IsReleased,
            episode.Channel,
            episode.Month,
            episode.Year,
            episode.RottenTomatoesId,
            series.GetPosterUrl(episode));

    private static Season ToSeason(this SeasonRequest request) =>
        new(
            Id.ForNullable<Season>(request.Id),
            request.ToTitles(),
            request.WatchStatus,
            request.ReleaseStatus,
            request.Channel,
            request.SequenceNumber,
            request.Periods.Select(ToPeriod));

    private static Period ToPeriod(this PeriodRequest request) =>
        new(
            Id.ForNullable<Period>(request.Id),
            request.StartMonth,
            request.StartYear,
            request.EndMonth,
            request.EndYear,
            request.IsSingleDayRelease,
            request.EpisodeCount)
        {
            RottenTomatoesId = request.RottenTomatoesId
        };

    private static SpecialEpisode ToSpecialEpisode(this SpecialEpisodeRequest request) =>
        new(
            Id.ForNullable<SpecialEpisode>(request.Id),
            request.ToTitles(),
            request.Month,
            request.Year,
            request.IsWatched,
            request.IsReleased,
            request.Channel,
            request.SequenceNumber)
        {
            RottenTomatoesId = request.RottenTomatoesId
        };

    private static void UpdateSeasons(this Series series, IReadOnlyCollection<SeasonRequest> requests)
    {
        var requestIds = requests.Select(req => req.Id).WhereValueNotNull().ToHashSet();

        foreach (var season in series.Seasons.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
        {
            series.RemoveSeason(season);
        }

        foreach (var season in series.Seasons.Where(e => requestIds.Contains(e.Id.Value)))
        {
            season.Update(requests.First(req => req.Id == season.Id.Value));
        }

        foreach (var request in requests.Where(req => !req.Id.HasValue))
        {
            series.AddSeason(request.ToSeason());
        }
    }

    private static void UpdateSpecialEpisodes(this Series series, IReadOnlyCollection<SpecialEpisodeRequest> requests)
    {
        var requestIds = requests.Select(req => req.Id).WhereValueNotNull().ToHashSet();

        foreach (var episode in series.SpecialEpisodes.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
        {
            series.RemoveSpecialEpisode(episode);
        }

        foreach (var episode in series.SpecialEpisodes.Where(e => requestIds.Contains(e.Id.Value)))
        {
            episode.Update(requests.First(req => req.Id == episode.Id.Value));
        }

        foreach (var request in requests.Where(req => !req.Id.HasValue))
        {
            series.AddSpecialEpisode(request.ToSpecialEpisode());
        }
    }

    private static void Update(this Season season, SeasonRequest request)
    {
        season.ReplaceTitles(
            request.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: false);

        season.ReplaceTitles(
            request.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: true);

        season.WatchStatus = request.WatchStatus;
        season.ReleaseStatus = request.ReleaseStatus;
        season.Channel = request.Channel;
        season.SequenceNumber = request.SequenceNumber;

        var requestIds = request.Periods.Select(req => req.Id).WhereValueNotNull().ToHashSet();

        foreach (var period in season.Periods.Where(e => !requestIds.Contains(e.Id.Value)).ToList())
        {
            season.RemovePeriod(period);
        }

        foreach (var period in season.Periods.Where(e => requestIds.Contains(e.Id.Value)))
        {
            period.Update(request.Periods.First(req => req.Id == period.Id.Value));
        }

        foreach (var periodRequest in request.Periods.Where(req => !req.Id.HasValue))
        {
            season.AddPeriod(periodRequest.ToPeriod());
        }
    }

    private static void Update(this Period period, PeriodRequest request)
    {
        period.StartMonth = request.StartMonth;
        period.StartYear = request.StartYear;
        period.EndMonth = request.EndMonth;
        period.EndYear = request.EndYear;
        period.IsSingleDayRelease = request.IsSingleDayRelease;
        period.EpisodeCount = request.EpisodeCount;
        period.RottenTomatoesId = request.RottenTomatoesId;
    }

    private static void Update(this SpecialEpisode episode, SpecialEpisodeRequest request)
    {
        episode.ReplaceTitles(
            request.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: false);

        episode.ReplaceTitles(
            request.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: true);

        episode.Month = request.Month;
        episode.Year = request.Year;
        episode.IsWatched = request.IsWatched;
        episode.IsReleased = request.IsReleased;
        episode.Channel = request.Channel;
        episode.SequenceNumber = request.SequenceNumber;
        episode.RottenTomatoesId = request.RottenTomatoesId;
    }

    private static string? GetPosterUrl(this Series series) =>
        series.PosterHash is not null ? $"/api/series/{series.Id.Value}/poster/?h={series.PosterHash}" : null;

    private static string? GetPosterUrl(this Series series, Period period) =>
        period.PosterHash is not null
            ? $"/api/series/{series.Id.Value}/seasons/periods/{period.Id.Value}/poster/?h={period.PosterHash}"
            : null;

    private static string? GetPosterUrl(this Series series, SpecialEpisode episode) =>
        episode.PosterHash is not null
            ? $"/api/series/{series.Id.Value}/special-episodes/{episode.Id.Value}/poster/?h={episode.PosterHash}"
            : null;
}
