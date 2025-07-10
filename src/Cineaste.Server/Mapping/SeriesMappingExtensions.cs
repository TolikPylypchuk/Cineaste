namespace Cineaste.Server.Mapping;

public static class SeriesMappingExtensions
{
    public static SeriesModel ToSeriesModel(this Series series) =>
        new(
            series.Id.Value,
            series.Titles.ToTitleModels(isOriginal: false),
            series.Titles.ToTitleModels(isOriginal: true),
            series.WatchStatus,
            series.ReleaseStatus,
            series.Kind.ToListKindModel(),
            [.. series.Seasons.Select(season => season.ToSeasonModel())],
            [.. series.SpecialEpisodes.Select(episode => episode.ToSpecialEpisodeModel())],
            series.ImdbId,
            series.RottenTomatoesId,
            series.FranchiseItem.GetDisplayNumber());

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
            request.Value.Titles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: false);

        series.ReplaceTitles(
            request.Value.OriginalTitles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: true);

        series.WatchStatus = request.Value.WatchStatus;
        series.ReleaseStatus = request.Value.ReleaseStatus;
        series.Kind = kind;
        series.ImdbId = request.Value.ImdbId;
        series.RottenTomatoesId = request.Value.RottenTomatoesId;

        series.UpdateSeasons(request.Value.Seasons);
        series.UpdateSpecialEpisodes(request.Value.SpecialEpisodes);
    }

    private static SeasonModel ToSeasonModel(this Season season) =>
        new(
            season.Id.Value,
            season.Titles.ToTitleModels(isOriginal: false),
            season.Titles.ToTitleModels(isOriginal: true),
            season.SequenceNumber,
            season.WatchStatus,
            season.ReleaseStatus,
            season.Channel,
            [.. season.Periods
                .Select(period => period.ToPeriodModel())
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .ThenBy(period => period.EndYear)
                .ThenBy(period => period.EndMonth)]);

    private static PeriodModel ToPeriodModel(this Period period) =>
        new(
            period.Id.Value,
            period.StartMonth,
            period.StartYear,
            period.EndMonth,
            period.EndYear,
            period.EpisodeCount,
            period.IsSingleDayRelease,
            period.RottenTomatoesId);

    private static SpecialEpisodeModel ToSpecialEpisodeModel(this SpecialEpisode episode) =>
        new(
            episode.Id.Value,
            episode.Titles.ToTitleModels(isOriginal: false),
            episode.Titles.ToTitleModels(isOriginal: true),
            episode.SequenceNumber,
            episode.IsWatched,
            episode.IsReleased,
            episode.Channel,
            episode.Month,
            episode.Year,
            episode.RottenTomatoesId);

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
            request.Titles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: false);

        season.ReplaceTitles(
            request.OriginalTitles.OrderBy(title => title.Priority).Select(title => title.Name),
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
            request.Titles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: false);

        episode.ReplaceTitles(
            request.OriginalTitles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: true);

        episode.Month = request.Month;
        episode.Year = request.Year;
        episode.IsWatched = request.IsWatched;
        episode.IsReleased = request.IsReleased;
        episode.Channel = request.Channel;
        episode.SequenceNumber = request.SequenceNumber;
        episode.RottenTomatoesId = request.RottenTomatoesId;
    }
}
