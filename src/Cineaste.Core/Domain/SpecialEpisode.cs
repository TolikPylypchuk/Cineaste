namespace Cineaste.Core.Domain;

public sealed class SpecialEpisode : TitledEntity<SpecialEpisode>
{
    private string channel;

    public Month Month { get; set; }

    public int Year
    {
        get;
        set => field = Require.Positive(value);
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
        get;
        set => field = Require.Positive(value);
    }

    public RottenTomatoesId? RottenTomatoesId { get; set; }

    public PosterHash? PosterHash { get; set; }

    public SpecialEpisode(
        Id<SpecialEpisode> id,
        IEnumerable<Title> titles,
        Month month,
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
