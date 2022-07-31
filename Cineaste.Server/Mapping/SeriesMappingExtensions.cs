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
            series.IsMiniseries,
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
}
