namespace Cineaste.Client.FormModels;

public sealed class PeriodFormModel : FormModel<PeriodModel>
{
    private DateOnly defaultDate;

    public int StartMonth { get; set; }
    public int StartYear { get; set; }

    public int EndMonth { get; set; }
    public int EndYear { get; set; }

    public int EpisodeCount { get; set; }
    public bool IsSingleDayRelease { get; set; }

    public string RottenTomatoesId { get; set; }

    public PeriodFormModel()
        : this(DateOnly.FromDateTime(DateTime.Now))
    { }

    public PeriodFormModel(DateOnly date)
    {
        this.defaultDate = date;

        this.StartMonth = date.Month;
        this.StartYear = date.Year;

        this.EndMonth = date.Month;
        this.EndYear = date.Year;

        this.EpisodeCount = 1;
        this.RottenTomatoesId = String.Empty;
    }

    public PeriodRequest ToRequest() =>
        new(
            this.BackingModel?.Id,
            this.StartMonth,
            this.StartYear,
            this.EndMonth,
            this.EndYear,
            this.EpisodeCount,
            this.IsSingleDayRelease,
            this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var period = this.BackingModel;

        this.StartMonth = period?.StartMonth ?? this.defaultDate.Month;
        this.StartYear = period?.StartYear ?? this.defaultDate.Year;

        this.EndMonth = period?.EndMonth ?? this.defaultDate.Month;
        this.EndYear = period?.EndYear ?? this.defaultDate.Year;

        this.EpisodeCount = period?.EpisodeCount ?? 1;
        this.IsSingleDayRelease = period?.IsSingleDayRelease ?? false;

        this.RottenTomatoesId = period?.RottenTomatoesId ?? String.Empty;
    }
}
