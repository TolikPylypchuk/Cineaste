namespace Cineaste.Client.FormModels;

using System.ComponentModel;

public sealed class SeasonFormModel : SeriesComponentFormModelBase<SeasonRequest, SeasonModel>
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

    public SeasonFormModel(
        string title,
        string originalTitle,
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber)
        : this(
              title,
              originalTitle,
              DateOnly.FromDateTime(DateTime.Now),
              channel,
              getSequenceNumber,
              lastSequenceNumber)
    { }

    public SeasonFormModel(
        string title,
        string originalTitle,
        DateOnly date,
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber)
        : base(getSequenceNumber, lastSequenceNumber)
    {
        var period = new PeriodFormModel(date);
        period.PropertyChanged += this.OnPeriodUpdated;

        this.periods = new() { period };
        this.Periods = new(this.periods);

        this.defaultTitle = title;
        this.Titles.Clear();
        this.Titles.Add(title);

        this.defaultOriginalTitle = originalTitle;
        this.OriginalTitles.Clear();
        this.OriginalTitles.Add(originalTitle);

        this.Channel = this.defaultChannel = channel;
        this.SequenceNumber = this.GetSequenceNumber();

        this.defaultDate = date;

        this.FinishInitialization();

    }

    public void AddNewPeriod()
    {
        var lastPeriod = this.periods
            .OrderByDescending(period => period.EndYear)
            .ThenByDescending(period => period.EndMonth)
            .First();

        var period = new PeriodFormModel(new DateOnly(lastPeriod.EndYear + 1, lastPeriod.StartMonth, 1));
        period.PropertyChanged += this.OnPeriodUpdated;

        this.periods.Add(period);

        this.UpdateRequest();
    }

    public void RemovePeriod(PeriodFormModel period)
    {
        if (this.periods.Count > 1)
        {
            this.periods.Remove(period);
            period.PropertyChanged -= this.OnPeriodUpdated;

            this.UpdateRequest();
        }
    }

    public override void UpdateRequest()
    {
        base.UpdateRequest();

        foreach (var period in this.periods)
        {
            period.UpdateRequest();
        }
    }

    public override SeasonRequest CreateRequest() =>
        new(
            this.BackingModel?.Id,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.SequenceNumber,
            this.WatchStatus,
            this.ReleaseStatus,
            this.Channel,
            this.Periods.Select(period => period.CreateRequest()).ToImmutableList());

    protected override void CopyFromModel()
    {
        var season = this.BackingModel;

        this.CopyTitles(season, this.defaultTitle, this.defaultOriginalTitle);

        this.WatchStatus = season?.WatchStatus ?? SeasonWatchStatus.NotWatched;
        this.ReleaseStatus = season?.ReleaseStatus ?? SeasonReleaseStatus.NotStarted;
        this.Channel = season?.Channel ?? this.defaultChannel;
        this.SequenceNumber = season?.SequenceNumber ?? this.GetSequenceNumber();

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

    private void OnPeriodUpdated(object? sender, PropertyChangedEventArgs e)
    {
        this.UpdateRequest();
        this.OnPropertyChanged(nameof(this.Periods));
    }
}
