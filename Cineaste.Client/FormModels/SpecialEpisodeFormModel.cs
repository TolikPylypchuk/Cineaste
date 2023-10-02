namespace Cineaste.Client.FormModels;

public sealed class SpecialEpisodeFormModel : SeriesComponentFormModelBase<SpecialEpisodeRequest, SpecialEpisodeModel>
{
    private readonly DateTime defaultDate;
    private readonly string defaultChannel;

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public DateTime? Date { get; set; }

    public string RottenTomatoesId { get; set; } = String.Empty;

    public int Month =>
        (this.Date ?? this.defaultDate).Month;

    public int Year =>
        (this.Date ?? this.defaultDate).Year;

    public override string Years =>
        this.Date?.Year.ToString() ?? String.Empty;

    public SpecialEpisodeFormModel(
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber)
        : this(DateOnly.FromDateTime(DateTime.Now), channel, getSequenceNumber, lastSequenceNumber)
    { }

    public SpecialEpisodeFormModel(
        DateOnly date,
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber)
        : base(getSequenceNumber, lastSequenceNumber)
    {
        this.defaultDate = date.ToDateTime(TimeOnly.MinValue);

        this.Date = this.defaultDate;
        this.Channel = this.defaultChannel = channel;
        this.SequenceNumber = this.GetSequenceNumber();

        this.FinishInitialization();
    }

    public override SpecialEpisodeRequest CreateRequest() =>
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

        this.Date = episode?.Month is not null && episode?.Year is not null
            ? new DateTime(episode.Year, episode.Month, 1)
            : this.defaultDate;

        this.RottenTomatoesId = episode?.RottenTomatoesId ?? String.Empty;
        this.SequenceNumber = episode?.SequenceNumber ?? this.GetSequenceNumber();
    }
}
