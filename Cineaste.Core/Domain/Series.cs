namespace Cineaste.Core.Domain;

public enum SeriesWatchStatus
{
    NotWatched,
    Watching,
    Watched,
    StoppedWatching
}

public enum SeriesReleaseStatus
{
    NotStarted,
    Running,
    Finished,
    Cancelled,
    Unknown
}

public sealed class Series : DomainObject
{
    public bool IsMiniseries { get; set; }

    public SeriesWatchStatus WatchStatus { get; set; } = SeriesWatchStatus.NotWatched;
    public SeriesReleaseStatus ReleaseStatus { get; set; } = SeriesReleaseStatus.NotStarted;

    public string? ImdbId { get; set; }
    public string? RottenTomatoesLink { get; set; }
    public string? PosterUrl { get; set; }

    public SeriesKind Kind { get; set; } = null!;

    public FranchiseItem? FranchiseItem { get; set; }

    public List<Title> Titles { get; set; } = new();

    public List<Season> Seasons { get; set; } = new();

    public List<SpecialEpisode> SpecialEpisodes { get; set; } = new();

    public HashSet<Tag> Tags { get; set; } = new();

    public CineasteList OwnerList { get; set; } = null!;
}
