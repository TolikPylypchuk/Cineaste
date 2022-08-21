namespace Cineaste.Client.FormModels;

public sealed class SeasonFormModel : SeriesComponentFormModel<SeasonModel>
{
    private readonly ObservableCollection<PeriodFormModel> periods;

    private readonly string defaultTitle;
    private readonly string defaultOriginalTitle;
    private readonly DateOnly defaultDate;
    private readonly string defaultChannel;

    public SeasonWatchStatus WatchStatus { get; set; }
    public SeasonReleaseStatus ReleaseStatus { get; set; }

    public ReadOnlyObservableCollection<PeriodFormModel> Periods { get; }

    public override string Years
    {
        get
        {
            int minYear = this.Periods.Min(p => p.StartYear);
            int maxYear = this.Periods.Max(p => p.EndYear);
            return minYear == maxYear ? minYear.ToString() : $"{minYear}-{maxYear}";
        }
    }

    public SeasonFormModel(string title, string originalTitle, string channel, Func<int> nextSequenceNumber)
        : this(title, originalTitle, DateOnly.FromDateTime(DateTime.Now), channel, nextSequenceNumber)
    { }

    public SeasonFormModel(
        string title,
        string originalTitle,
        DateOnly date,
        string channel,
        Func<int> nextSequenceNumber)
        : base(nextSequenceNumber)
    {
        this.defaultTitle = title;
        this.Titles.Clear();
        this.Titles.Add(title);

        this.defaultOriginalTitle = originalTitle;
        this.OriginalTitles.Clear();
        this.OriginalTitles.Add(originalTitle);

        this.Channel = this.defaultChannel = channel;
        this.SequenceNumber = nextSequenceNumber();

        this.defaultDate = date;

        this.periods = new() { new(date) };
        this.Periods = new(this.periods);
    }

    public void AddNewPeriod()
    {
        var lastPeriod = this.periods
            .OrderByDescending(period => period.EndYear)
            .ThenByDescending(period => period.EndMonth)
            .First();

        this.periods.Add(new PeriodFormModel(new DateOnly(lastPeriod.EndYear + 1, lastPeriod.StartMonth, 1)));
    }

    public void RemovePeriod(PeriodFormModel period)
    {
        if (this.periods.Count > 1)
        {
            this.periods.Remove(period);
        }
    }

    public SeasonRequest ToRequest() =>
        new(
            this.BackingModel?.Id,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.SequenceNumber,
            this.WatchStatus,
            this.ReleaseStatus,
            this.Channel,
            this.Periods.Select(period => period.ToRequest()).ToImmutableList());

    protected override void CopyFromModel()
    {
        var season = this.BackingModel;

        this.CopyTitles(season, this.defaultTitle, this.defaultOriginalTitle);

        this.WatchStatus = season?.WatchStatus ?? SeasonWatchStatus.NotWatched;
        this.ReleaseStatus = season?.ReleaseStatus ?? SeasonReleaseStatus.NotStarted;
        this.Channel = season?.Channel ?? this.defaultChannel;
        this.SequenceNumber = season?.SequenceNumber ?? this.NextSequenceNumber();

        this.periods.Clear();

        if (season is not null)
        {
            foreach (var period in season.Periods)
            {
                var form = new PeriodFormModel();
                form.CopyFrom(period);
                this.periods.Add(form);
            }
        } else
        {
            this.periods.Add(new(this.defaultDate));
        }
    }
}
