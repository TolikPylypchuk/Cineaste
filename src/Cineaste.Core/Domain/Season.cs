namespace Cineaste.Core.Domain;

public sealed class Season : TitledEntity<Season>
{
    private string channel;
    private readonly List<SeasonPart> parts;

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
        get;
        set => field = Require.Positive(value);
    }

    public IReadOnlyCollection<SeasonPart> Parts =>
        this.parts.AsReadOnly();

    public int StartYear =>
        this.Parts
            .Select(part => part.Period)
            .OrderBy(period => period.StartYear)
            .ThenBy(period => period.StartMonth)
            .First()
            .StartYear;

    public int EndYear =>
        this.Parts
            .Select(part => part.Period)
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
        IEnumerable<SeasonPart> parts)
        : base(id, titles)
    {
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;

        this.parts = [.. parts];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Season(Id<Season> id)
        : base(id)
    {
        this.channel = null!;
        this.parts = [];
    }

    public void AddPart(SeasonPart period) =>
        this.parts.Add(period);

    public void RemovePart(SeasonPart period) =>
        this.parts.Remove(period);
}
