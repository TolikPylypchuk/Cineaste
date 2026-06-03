namespace Cineaste.Common;

public enum SeriesWatchStatus
{
    NotWatched,
    Watching,
    Hiatus,
    Watched,
    StoppedWatching
}

public enum SeriesReleaseStatus
{
    NotStarted,
    Running,
    Hiatus,
    Finished,
    Cancelled,
    Unknown
}

public enum SeasonWatchStatus
{
    NotWatched,
    Watching,
    Hiatus,
    Watched,
    StoppedWatching
}

public enum SeasonReleaseStatus
{
    NotStarted,
    Running,
    Hiatus,
    Finished,
    Unknown
}

public static class SeriesEnumExtensions
{
    extension(SeriesWatchStatus seriesWatchStatus)
    {
        public SeasonWatchStatus ToSeasonWatchStatus() =>
            seriesWatchStatus switch
            {
                SeriesWatchStatus.NotWatched => SeasonWatchStatus.NotWatched,
                SeriesWatchStatus.Watching => SeasonWatchStatus.Watching,
                SeriesWatchStatus.Hiatus => SeasonWatchStatus.Hiatus,
                SeriesWatchStatus.Watched => SeasonWatchStatus.Watched,
                SeriesWatchStatus.StoppedWatching => SeasonWatchStatus.StoppedWatching,
                _ => throw new ArgumentOutOfRangeException(nameof(seriesWatchStatus))
            };
    }

    extension(SeriesReleaseStatus seriesReleaseStatus)
    {
        public SeasonReleaseStatus ToSeasonReleaseStatus() =>
            seriesReleaseStatus switch
            {
                SeriesReleaseStatus.NotStarted => SeasonReleaseStatus.NotStarted,
                SeriesReleaseStatus.Running => SeasonReleaseStatus.Running,
                SeriesReleaseStatus.Hiatus => SeasonReleaseStatus.Hiatus,
                SeriesReleaseStatus.Finished or SeriesReleaseStatus.Cancelled => SeasonReleaseStatus.Finished,
                SeriesReleaseStatus.Unknown => SeasonReleaseStatus.Unknown,
                _ => throw new ArgumentOutOfRangeException(nameof(seriesReleaseStatus))
            };
    }
}
