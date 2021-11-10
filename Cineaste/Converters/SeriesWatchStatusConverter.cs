namespace Cineaste.Converters;

public sealed class SeriesWatchStatusConverter : EnumConverter<SeriesWatchStatus>
{
    protected override Dictionary<SeriesWatchStatus, string> CreateConverterDictionary() =>
        new()
        {
            [SeriesWatchStatus.NotWatched] = Messages.SeriesNotWatched,
            [SeriesWatchStatus.Watching] = Messages.SeriesWatching,
            [SeriesWatchStatus.Watched] = Messages.SeriesWatched,
            [SeriesWatchStatus.StoppedWatching] = Messages.SeriesStoppedWatching
        };
}
