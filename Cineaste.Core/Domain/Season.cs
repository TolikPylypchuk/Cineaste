namespace Cineaste.Core.Domain;

public sealed class Season : TitledEntity<Season>
{
    private string channel;
    private int sequenceNumber;
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

    public IReadOnlyCollection<Period> Periods =>
        this.periods.AsReadOnly();

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
        : base(id, titles)
    {
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;

        this.periods = periods.ToList();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Season(Id<Season> id)
        : base(id)
    {
        this.channel = null!;
        this.periods = new();
    }

    public void AddPeriod(Period period) =>
        this.periods.Add(period);

    public void RemovePeriod(Period period) =>
        this.periods.Remove(period);
}
