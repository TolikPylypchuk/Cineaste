namespace Cineaste.Client.FormModels;

public sealed class SeasonFormModel : SeriesComponentFormModel<SeasonModel, SeasonRequest>
{
    private readonly Func<int> getDefaultSequenceNumber;
    private readonly ObservableCollection<PeriodFormModel> periods;

    private readonly string defaultTitle;
    private readonly string defaultOriginalTitle;
    private readonly DateTime defaultDate;

    public SeasonWatchStatus WatchStatus { get; set; }
    public SeasonReleaseStatus ReleaseStatus { get; set; }

    public string Channel { get; set; } = String.Empty;

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

    public SeasonFormModel(string title, string originalTitle, Func<int> getDefaultSequenceNumber)
        : this(title, originalTitle, DateTime.Now, getDefaultSequenceNumber)
    { }

    public SeasonFormModel(string title, string originalTitle, DateTime date, Func<int> getDefaultSequenceNumber)
    {
        this.defaultTitle = title;
        this.Titles.Clear();
        this.Titles.Add(title);

        this.defaultOriginalTitle = originalTitle;
        this.OriginalTitles.Clear();
        this.Titles.Add(originalTitle);

        this.defaultDate = date;
        this.getDefaultSequenceNumber = getDefaultSequenceNumber;

        this.periods = new() { new(date) };
        this.Periods = new(this.periods);
    }

    public void AddNewPeriod()
    {
        var lastPeriod = this.periods
            .OrderByDescending(period => period.EndYear)
            .ThenByDescending(period => period.EndMonth)
            .First();

        this.periods.Add(new PeriodFormModel(new DateTime(lastPeriod.EndYear + 1, lastPeriod.StartMonth, 1)));
    }

    public void RemovePeriod(PeriodFormModel period)
    {
        if (this.periods.Count > 1)
        {
            this.periods.Remove(period);
        }
    }

    protected override void CopyFromModel()
    {
        var season = this.BackingModel;

        this.CopyTitles(season);

        if (this.Titles.Count == 0)
        {
            this.Titles.Add(this.defaultTitle);
        }

        if (this.OriginalTitles.Count == 0)
        {
            this.OriginalTitles.Add(this.defaultOriginalTitle);
        }

        this.WatchStatus = season?.WatchStatus ?? SeasonWatchStatus.NotWatched;
        this.ReleaseStatus = season?.ReleaseStatus ?? SeasonReleaseStatus.NotStarted;
        this.Channel = season?.Channel ?? String.Empty;
        this.SequenceNumber = season?.SequenceNumber ?? this.getDefaultSequenceNumber();

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
