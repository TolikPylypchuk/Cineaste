namespace Cineaste.Core.Domain;

public sealed class Season : Entity<Season>
{
    private string channel;
    private int sequenceNumber;
    private readonly List<Title> titles;
    private readonly List<Period> periods;

    public SeasonWatchStatus WatchStatus { get; set; }
    public SeasonReleaseStatus ReleaseStatus { get; set; }

    public string Channel
    {
        get => this.channel;

        [MemberNotNull(nameof(channel))]
        set => this.channel = Require.NotBlank(value);
    }

    public int SequenceNumber
    {
        get => this.sequenceNumber;
        set => this.sequenceNumber = Require.Positive(value);
    }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public IReadOnlyCollection<Period> Periods =>
        this.periods.AsReadOnly();

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
        this.Periods
            .OrderBy(period => period.StartYear)
            .ThenBy(period => period.StartMonth)
            .First()
            .StartYear;

    public int EndYear =>
        this.Periods
            .OrderByDescending(period => period.EndYear)
            .ThenByDescending(period => period.EndMonth)
            .First()
            .EndYear;

    public Season(
        Id<Season> id,
        IEnumerable<Title> titles,
        SeasonWatchStatus watchStatus,
        SeasonReleaseStatus releaseStatus,
        string channel,
        int sequenceNumber,
        IEnumerable<Period> periods)
        : base(id)
    {
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;

        this.titles = titles.ToList();
        this.periods = periods.ToList();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Season(Id<Season> id)
        : base(id)
    {
        this.channel = null!;
        this.titles = new();
        this.periods = new();
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
