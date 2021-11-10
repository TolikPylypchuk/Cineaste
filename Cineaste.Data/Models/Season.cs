namespace Cineaste.Data.Models;

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

[Table("Seasons")]
public sealed class Season : EntityBase
{
    public SeasonWatchStatus WatchStatus { get; set; } = SeasonWatchStatus.NotWatched;
    public SeasonReleaseStatus ReleaseStatus { get; set; } = SeasonReleaseStatus.NotStarted;

    public string Channel { get; set; } = String.Empty;

    public int SequenceNumber { get; set; }

    public int SeriesId { get; set; }

    [Write(false)]
    public Series Series { get; set; } = null!;

    [Write(false)]
    public List<Title> Titles { get; set; } = new();

    [Write(false)]
    public List<Period> Periods { get; set; } = new();

    [Computed]
    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    [Computed]
    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    [Computed]
    public int StartYear =>
        this.Periods
            .OrderBy(period => period.StartYear)
            .ThenBy(period => period.StartMonth)
            .First()
            .StartYear;

    [Computed]
    public int EndYear =>
        this.Periods
            .OrderByDescending(period => period.EndYear)
            .ThenByDescending(period => period.EndMonth)
            .First()
            .EndYear;

    public override string ToString() =>
        $"Season #{this.Id}: {Title.ToString(this.Titles)} ({this.Channel})";
}
