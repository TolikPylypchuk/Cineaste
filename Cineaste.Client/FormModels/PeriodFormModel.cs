namespace Cineaste.Client.FormModels;

public sealed class PeriodFormModel : FormModelBase<PeriodRequest, PeriodModel>
{
    private readonly DateTime defaultDate;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int StartMonth =>
        (this.StartDate ?? this.defaultDate).Month;

    public int StartYear =>
        (this.StartDate ?? this.defaultDate).Year;

    public int EndMonth =>
        (this.EndDate ?? this.defaultDate).Month;

    public int EndYear =>
        (this.EndDate ?? this.defaultDate).Year;

    public int EpisodeCount { get; set; }
    public bool IsSingleDayRelease { get; set; }

    public string RottenTomatoesId { get; set; }

    public PeriodFormModel()
        : this(DateOnly.FromDateTime(DateTime.Now))
    { }

    public PeriodFormModel(DateOnly date)
    {
        this.defaultDate = date.ToDateTime(TimeOnly.MinValue);
        this.StartDate = this.defaultDate;
        this.EndDate = this.defaultDate;

        this.EpisodeCount = 1;
        this.RottenTomatoesId = String.Empty;

        this.FinishInitialization();
    }

    public override PeriodRequest CreateRequest() =>
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

        this.StartDate = period?.StartMonth is not null && period?.StartYear is not null
            ? new DateTime(period.StartYear, period.StartMonth, 1)
            : this.defaultDate;

        this.EndDate = period?.EndMonth is not null && period?.EndYear is not null
            ? new DateTime(period.EndYear, period.EndMonth, 1)
            : this.defaultDate;

        this.EpisodeCount = period?.EpisodeCount ?? 1;
        this.IsSingleDayRelease = period?.IsSingleDayRelease ?? false;

        this.RottenTomatoesId = period?.RottenTomatoesId ?? String.Empty;
    }
}
