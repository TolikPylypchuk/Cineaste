namespace Cineaste.Client.FormModels;

public sealed class SpecialEpisodeFormModel : SeriesComponentFormModelBase<SpecialEpisodeRequest, SpecialEpisodeModel>
{
    private readonly DateOnly defaultDate;
    private readonly string defaultChannel;

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public string RottenTomatoesId { get; set; } = String.Empty;

    public override string Years =>
        this.Year.ToString();

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
        this.defaultDate = date;
        this.Month = date.Month;
        this.Year = date.Year;
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

        this.Month = episode?.Month ?? this.defaultDate.Month;
        this.Year = episode?.Year ?? this.defaultDate.Year;

        this.RottenTomatoesId = episode?.RottenTomatoesId ?? String.Empty;
        this.SequenceNumber = episode?.SequenceNumber ?? this.GetSequenceNumber();
    }
}
