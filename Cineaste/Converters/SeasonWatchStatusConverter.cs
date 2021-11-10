namespace Cineaste.Converters;

public sealed class SeasonWatchStatusConverter : EnumConverter<SeasonWatchStatus>
{
    protected override Dictionary<SeasonWatchStatus, string> CreateConverterDictionary() =>
        new()
        {
            [SeasonWatchStatus.NotWatched] = Messages.SeasonNotWatched,
            [SeasonWatchStatus.Watching] = Messages.SeasonWatching,
            [SeasonWatchStatus.Hiatus] = Messages.SeasonHiatus,
            [SeasonWatchStatus.Watched] = Messages.SeasonWatched,
            [SeasonWatchStatus.StoppedWatching] = Messages.SeasonStoppedWatching
        };
}
