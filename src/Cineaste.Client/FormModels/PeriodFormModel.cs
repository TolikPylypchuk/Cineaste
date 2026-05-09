namespace Cineaste.Client.FormModels;

public sealed class PeriodFormModel : FormModelBase<SeasonPartRequest, SeasonPartModel>
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

    public override SeasonPartRequest CreateRequest() =>
        new(
            this.BackingModel?.Id,
            new ReleasePeriodRequest(
                this.StartMonth,
                this.StartYear,
                this.EndMonth,
                this.EndYear,
                this.EpisodeCount,
                this.IsSingleDayRelease),
            this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var part = this.BackingModel;

        this.StartDate = part?.Period.StartMonth is not null && part?.Period.StartYear is not null
            ? new DateTime(part.Period.StartYear, part.Period.StartMonth, 1)
            : this.defaultDate;

        this.EndDate = part?.Period.EndMonth is not null && part?.Period.EndYear is not null
            ? new DateTime(part.Period.EndYear, part.Period.EndMonth, 1)
            : this.defaultDate;

        this.EpisodeCount = part?.Period.EpisodeCount ?? 1;
        this.IsSingleDayRelease = part?.Period.IsSingleDayRelease ?? false;

        this.RottenTomatoesId = part?.RottenTomatoesId ?? String.Empty;
    }
}
