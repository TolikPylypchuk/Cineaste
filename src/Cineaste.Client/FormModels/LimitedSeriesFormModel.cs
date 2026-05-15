using System.ComponentModel;

namespace Cineaste.Client.FormModels;

public sealed class LimitedSeriesFormModel : TitledFormModelBase<LimitedSeriesRequest, LimitedSeriesModel>
{
    private readonly ListKindModel defaultKind;

    public ReleasePeriodFormModel Period { get; } = new();

    public SeriesWatchStatus WatchStatus { get; set; }
    public SeriesReleaseStatus ReleaseStatus { get; set; }

    public string Channel { get; set; } = String.Empty;

    public ListKindModel Kind { get; set; }

    public string ImdbId { get; set; } = String.Empty;
    public string RottenTomatoesId { get; set; } = String.Empty;
    public string RottenTomatoesSubId { get; set; } = String.Empty;

    public Guid? ParentFranchiseId { get; private set; }

    public int? SequenceNumber { get; private set; }

    public bool IsFirst { get; private set; }
    public bool IsLast { get; private set; }

    public LimitedSeriesFormModel(ListKindModel kind, Guid? parentFranchiseId)
    {
        ArgumentNullException.ThrowIfNull(kind);

        this.defaultKind = kind;
        this.Kind = this.defaultKind;
        this.ParentFranchiseId = parentFranchiseId;

        this.Period.PropertyChanged += this.OnPeriodUpdated;

        this.FinishInitialization();
    }

    public override LimitedSeriesRequest CreateRequest() =>
        new(
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.Period.CreateRequest(),
            this.WatchStatus,
            this.ReleaseStatus,
            this.Channel,
            this.Kind.Id,
            this.ImdbId,
            this.RottenTomatoesId,
            this.RottenTomatoesSubId,
            this.ParentFranchiseId);

    protected override void CopyFromModel()
    {
        var limitedSeries = this.BackingModel;

        this.CopyTitles(limitedSeries);

        this.Period.CopyFrom(limitedSeries?.Period);

        this.WatchStatus = limitedSeries?.WatchStatus ?? SeriesWatchStatus.NotWatched;
        this.ReleaseStatus = limitedSeries?.ReleaseStatus ?? SeriesReleaseStatus.NotStarted;
        this.Channel = limitedSeries?.Channel ?? String.Empty;
        this.Kind = limitedSeries?.Kind ?? this.defaultKind;
        this.ImdbId = limitedSeries?.ImdbId ?? String.Empty;
        this.RottenTomatoesId = limitedSeries?.RottenTomatoesId ?? String.Empty;
        this.RottenTomatoesSubId = limitedSeries?.RottenTomatoesSubId ?? String.Empty;

        if (limitedSeries?.FranchiseItem is { } franchiseItem)
        {
            this.ParentFranchiseId = franchiseItem.ParentFranchiseId;
            this.SequenceNumber = franchiseItem.SequenceNumber;
            this.IsFirst = franchiseItem.IsFirstInFranchise;
            this.IsLast = franchiseItem.IsLastInFranchise;
        }
    }

    private void OnPeriodUpdated(object? sender, PropertyChangedEventArgs e) =>
        this.UpdateRequest();
}
