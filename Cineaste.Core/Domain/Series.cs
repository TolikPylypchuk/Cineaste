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

    public Kind Kind { get; set; } = null!;

    public FranchiseItem? FranchiseItem { get; set; }

    public List<Title> Titles { get; set; } = new();

    public List<Season> Seasons { get; set; } = new();

    public List<SpecialEpisode> SpecialEpisodes { get; set; } = new();

    public HashSet<Tag> Tags { get; set; } = new();

    public MovieSeriesList OwnerList { get; set; } = null!;

    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public int StartYear =>
        Math.Min(
            this.Seasons
                .OrderBy(season => season.StartYear)
                .FirstOrDefault()
                ?.StartYear
                ?? Int32.MaxValue,
            this.SpecialEpisodes
                .OrderBy(episode => episode.Year)
                .ThenBy(episode => episode.Month)
                .FirstOrDefault()
                ?.Year
                ?? Int32.MaxValue);

    public int EndYear =>
        Math.Max(
            this.Seasons
                .OrderByDescending(season => season.EndYear)
                .FirstOrDefault()
                ?.EndYear
                ?? Int32.MinValue,
            this.SpecialEpisodes
                .OrderByDescending(episode => episode.Year)
                .ThenByDescending(episode => episode.Month)
                .FirstOrDefault()
                ?.Year
                ?? Int32.MinValue);

    public override string ToString() =>
        $"Series #{this.Id}: {Title.ToString(this.Titles)}";
}
