namespace Cineaste.Core.Domain;

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

public sealed class Season : DomainObject
{
    public SeasonWatchStatus WatchStatus { get; set; } = SeasonWatchStatus.NotWatched;
    public SeasonReleaseStatus ReleaseStatus { get; set; } = SeasonReleaseStatus.NotStarted;

    public string Channel { get; set; } = String.Empty;

    public int SequenceNumber { get; set; }

    public Series Series { get; set; } = null!;

    public List<Title> Titles { get; set; } = new();

    public List<Period> Periods { get; set; } = new();
}
