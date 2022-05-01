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

public sealed class Season : Entity<Season>
{
    private string channel;

    private readonly List<Title> titles;
    private readonly List<Period> periods;

    public SeasonWatchStatus WatchStatus { get; set; } = SeasonWatchStatus.NotWatched;
    public SeasonReleaseStatus ReleaseStatus { get; set; } = SeasonReleaseStatus.NotStarted;

    public string Channel
    {
        get => this.channel;

        [MemberNotNull(nameof(channel))]
        set => this.channel = Require.NotBlank(value);
    }

    public int SequenceNumber { get; set; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public IReadOnlyCollection<Period> Periods =>
        this.periods.AsReadOnly();

    public Season(
        Id<Season> id,
        SeasonWatchStatus watchStatus,
        SeasonReleaseStatus releaseStatus,
        string channel,
        int sequenceNumber,
        IEnumerable<Title> titles,
        IEnumerable<Period> periods)
        : base(id)
    {
        this.Channel = channel;
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;

        this.titles = titles.ToList();
        this.periods = periods.ToList();
    }

    public Title AddTitle(string name, bool isOriginal)
    {
        int priority = this.titles
            .Where(title => title.IsOriginal == isOriginal)
            .Max(title => title.Priority) + 1;

        var title = new Title(name, priority, isOriginal);

        this.titles.Add(title);

        return title;
    }

    public void RemoveTitle(string name, bool isOriginal) =>
        this.titles.RemoveAll(title => title.Name == name && title.IsOriginal == isOriginal);

    public void AddPeriod(Period period) =>
        this.periods.Add(period);

    public void RemovePeriod(Period period) =>
        this.periods.Remove(period);
}
