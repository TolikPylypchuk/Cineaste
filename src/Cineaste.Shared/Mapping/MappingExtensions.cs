namespace Cineaste.Shared.Mapping;

public static class MappingExtensions
{
    public static ListItemModel ToListItemModel(this MovieModel movie) =>
        new(
            movie.Id,
            ListItemType.Movie,
            true,
            movie.DisplayNumber,
            movie.Titles.First().Name,
            movie.OriginalTitles.First().Name,
            movie.Year,
            movie.Year,
            movie.IsWatched
                ? movie.Kind.WatchedColor
                : movie.IsReleased ? movie.Kind.NotWatchedColor : movie.Kind.NotReleasedColor,
            null);

    public static ListItemModel ToListItemModel(this SeriesModel series) =>
        new(
            series.Id,
            ListItemType.Series,
            true,
            series.DisplayNumber,
            series.Titles.First().Name,
            series.OriginalTitles.First().Name,
            series.StartYear,
            series.EndYear,
            series.WatchStatus != SeriesWatchStatus.NotWatched
                ? series.Kind.WatchedColor
                : series.ReleaseStatus != SeriesReleaseStatus.NotStarted
                    ? series.Kind.NotWatchedColor
                    : series.Kind.NotReleasedColor,
            null);
}
