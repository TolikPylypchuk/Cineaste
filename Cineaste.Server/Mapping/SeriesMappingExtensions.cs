namespace Cineaste.Server.Mapping;

public static class SeriesMappingExtensions
{
    public static SeriesModel ToSeriesModel(this Series series) =>
        new(
            series.Id.Value,
            series.Titles
                .Where(title => !title.IsOriginal)
                .Select(title => title.ToTitleModel())
                .OrderBy(title => title.Priority)
                .ToImmutableList(),
            series.Titles
                .Where(title => title.IsOriginal)
                .Select(title => title.ToTitleModel())
                .OrderBy(title => title.Priority)
                .ToImmutableList(),
            series.WatchStatus,
            series.ReleaseStatus,
            series.Kind.ToListKindModel(),
            series.IsMiniseries,
            series.ImdbId,
            series.RottenTomatoesLink,
            series.FranchiseItem.GetDisplayNumber());
}
