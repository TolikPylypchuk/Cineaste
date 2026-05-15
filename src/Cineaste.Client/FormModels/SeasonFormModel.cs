using System.ComponentModel;

namespace Cineaste.Client.FormModels;

public sealed class SeasonFormModel : SeriesComponentFormModelBase<SeasonRequest, SeasonModel>
{
    private readonly ObservableCollection<SeasonPartFormModel> parts;

    private readonly string defaultTitle;
    private readonly string defaultOriginalTitle;
    private readonly DateOnly defaultDate;
    private readonly string defaultChannel;

    public SeasonWatchStatus WatchStatus { get; set; }
    public SeasonReleaseStatus ReleaseStatus { get; set; }

    public ReadOnlyObservableCollection<SeasonPartFormModel> Parts { get; }

    public override string Years
    {
        get
        {
            int minYear = this.Parts.Min(p => p.Period.StartYear);
            int maxYear = this.Parts.Max(p => p.Period.EndYear);
            return minYear == maxYear ? minYear.ToString() : $"{minYear}-{maxYear}";
        }
    }

    public SeasonFormModel(
        string title,
        string originalTitle,
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber,
        Func<bool> canSetPoster)
        : this(
              title,
              originalTitle,
              DateOnly.FromDateTime(DateTime.Now),
              channel,
              getSequenceNumber,
              lastSequenceNumber,
              canSetPoster)
    { }

    public SeasonFormModel(
        string title,
        string originalTitle,
        DateOnly date,
        string channel,
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber,
        Func<bool> canSetPoster)
        : base(getSequenceNumber, lastSequenceNumber, canSetPoster)
    {
        var period = new SeasonPartFormModel(date);
        period.PropertyChanged += this.OnPeriodUpdated;

        this.parts = [period];
        this.Parts = new(this.parts);

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

    public void AddNewPart()
    {
        var lastPart = this.parts
            .OrderByDescending(part => part.Period.EndYear)
            .ThenByDescending(part => part.Period.EndMonth)
            .First();

        var part = new SeasonPartFormModel(new DateOnly(lastPart.Period.EndYear + 1, lastPart.Period.StartMonth, 1));
        part.PropertyChanged += this.OnPeriodUpdated;

        this.parts.Add(part);

        this.UpdateRequest();
        this.OnPropertyChanged(nameof(this.Parts));
    }

    public void RemovePart(SeasonPartFormModel part)
    {
        if (this.parts.Count > 1)
        {
            this.parts.Remove(part);
            part.PropertyChanged -= this.OnPeriodUpdated;

            this.UpdateRequest();
            this.OnPropertyChanged(nameof(this.Parts));
        }
    }

    public override void UpdateRequest()
    {
        base.UpdateRequest();

        foreach (var part in this.parts)
        {
            part.UpdateRequest();
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
            this.Parts.Select(period => period.CreateRequest()).ToImmutableList().AsValue());

    protected override void CopyFromModel()
    {
        var season = this.BackingModel;

        this.CopyTitles(season, this.defaultTitle, this.defaultOriginalTitle);

        this.WatchStatus = season?.WatchStatus ?? SeasonWatchStatus.NotWatched;
        this.ReleaseStatus = season?.ReleaseStatus ?? SeasonReleaseStatus.NotStarted;
        this.Channel = season?.Channel ?? this.defaultChannel;
        this.SequenceNumber = season?.SequenceNumber ?? this.GetSequenceNumber();

        foreach (var part in this.parts)
        {
            part.PropertyChanged -= this.OnPeriodUpdated;
        }

        this.parts.Clear();

        if (season is not null)
        {
            foreach (var period in season.Parts)
            {
                var form = new SeasonPartFormModel();
                form.CopyFrom(period);
                form.PropertyChanged += this.OnPeriodUpdated;
                this.parts.Add(form);
            }
        } else
        {
            var period = new SeasonPartFormModel(this.defaultDate);
            period.PropertyChanged += this.OnPeriodUpdated;
            this.parts.Add(period);
        }
    }

    private void OnPeriodUpdated(object? sender, PropertyChangedEventArgs e)
    {
        this.UpdateRequest();
        this.OnPropertyChanged(nameof(this.Parts));
    }
}
