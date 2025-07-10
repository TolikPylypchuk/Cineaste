namespace Cineaste.Core.Domain;

public sealed class SpecialEpisode : TitledEntity<SpecialEpisode>
{
    private int month;
    private int year;
    private string channel;
    private int sequenceNumber;
    private string? rottenTomatoesId;

    public int Month
    {
        get => this.month;
        set => this.month = Require.Month(value);
    }

    public int Year
    {
        get => this.year;
        set => this.year = Require.Positive(value);
    }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

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

    public string? RottenTomatoesId
    {
        get => this.rottenTomatoesId;
        set => this.rottenTomatoesId = Require.RottenTomatoesId(value);
    }

    public Poster? Poster { get; set; }

    public SpecialEpisode(
        Id<SpecialEpisode> id,
        IEnumerable<Title> titles,
        int month,
        int year,
        bool isWatched,
        bool isReleased,
        string channel,
        int sequenceNumber)
        : base(id, titles)
    {
        this.Month = month;
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SpecialEpisode(Id<SpecialEpisode> id)
        : base(id) =>
        this.channel = null!;
}
