namespace Cineaste.Client.FormModels;

public sealed class SpecialEpisodeFormModel : SeriesComponentFormModel<SpecialEpisodeModel>
{
    private readonly Func<int> getDefaultSequenceNumber;

    private readonly DateOnly defaultDate;
    private readonly string defaultChannel;

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public string RottenTomatoesId { get; set; } = String.Empty;

    public override string Years =>
        this.Year.ToString();

    public SpecialEpisodeFormModel(string channel, Func<int> getDefaultSequenceNumber)
        : this(DateOnly.FromDateTime(DateTime.Now), channel, getDefaultSequenceNumber)
    { }

    public SpecialEpisodeFormModel(DateOnly date, string channel, Func<int> getDefaultSequenceNumber)
    {
        this.defaultDate = date;
        this.Channel = this.defaultChannel = channel;
        this.SequenceNumber = getDefaultSequenceNumber();
        this.getDefaultSequenceNumber = getDefaultSequenceNumber;
    }

    public SpecialEpisodeRequest ToRequest() =>
        new(
            this.BackingModel?.Id,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.SequenceNumber,
            this.IsWatched,
            this.IsReleased,
            this.Channel,
            this.Month,
            this.Year,
            this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var episode = this.BackingModel;

        this.CopyTitles(episode);

        this.IsWatched = episode?.IsWatched ?? false;
        this.IsReleased = episode?.IsReleased ?? true;

        this.Channel = episode?.Channel ?? this.defaultChannel;

        this.Month = episode?.Month ?? this.defaultDate.Month;
        this.Year = episode?.Year ?? this.defaultDate.Year;

        this.RottenTomatoesId = episode?.RottenTomatoesId ?? String.Empty;
        this.SequenceNumber = episode?.SequenceNumber ?? this.getDefaultSequenceNumber();
    }
}
