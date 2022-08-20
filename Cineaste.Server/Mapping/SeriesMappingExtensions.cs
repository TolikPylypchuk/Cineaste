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
            series.Seasons.Select(season => season.ToSeasonModel()).ToImmutableList(),
            series.SpecialEpisodes.Select(episode => episode.ToSpecialEpisodeModel()).ToImmutableList(),
            series.ImdbId,
            series.RottenTomatoesId,
            series.FranchiseItem.GetDisplayNumber());

    public static SeasonModel ToSeasonModel(this Season season) =>
        new(
            season.Id.Value,
            season.Titles.ToTitleModels(isOriginal: false),
            season.Titles.ToTitleModels(isOriginal: true),
            season.SequenceNumber,
            season.WatchStatus,
            season.ReleaseStatus,
            season.Channel,
            season.Periods
                .Select(period => period.ToPeriodModel())
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .ThenBy(period => period.EndYear)
                .ThenBy(period => period.EndMonth)
                .ToImmutableList());

    public static PeriodModel ToPeriodModel(this Period period) =>
        new(
            period.Id.Value,
            period.StartMonth,
            period.StartYear,
            period.EndMonth,
            period.EndYear,
            period.EpisodeCount,
            period.IsSingleDayRelease,
            period.RottenTomatoesId);

    public static SpecialEpisodeModel ToSpecialEpisodeModel(this SpecialEpisode episode) =>
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

    public static Series ToSeries(this Validated<SeriesRequest> request, Id<Series> id, SeriesKind kind) =>
        new(
            id,
            request.Value.ToTitles(),
            request.Value.Seasons.Select(ToSeason),
            request.Value.SpecialEpisodes.Select(ToSpecialEpisode),
            request.Value.WatchStatus,
            request.Value.ReleaseStatus,
            kind);

    private static Season ToSeason(this SeasonRequest request) =>
        new(
            Id.CreateFromNullable<Season>(request.Id),
            request.ToTitles(),
            request.WatchStatus,
            request.ReleaseStatus,
            request.Channel,
            request.SequenceNumber,
            request.Periods.Select(ToPeriod));

    private static Period ToPeriod(this PeriodRequest request) =>
        new(
            Id.CreateFromNullable<Period>(request.Id),
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
            Id.CreateFromNullable<SpecialEpisode>(request.Id),
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
}
