namespace Cineaste.Client.FormModels;

public sealed class SeasonFormModel : SeriesComponentFormModel<SeasonModel, SeasonRequest>
{
    private readonly Func<int> getDefaultSequenceNumber;
    public readonly ObservableCollection<PeriodFormModel> periods = new() { new() };

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

    public SeasonFormModel(Func<int> getDefaultSequenceNumber)
    {
        this.getDefaultSequenceNumber = getDefaultSequenceNumber;
        this.Periods = new(this.periods);
    }

    public void AddNewPeriod() =>
        this.periods.Add(new());

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
            this.AddNewPeriod();
        }
    }
}
