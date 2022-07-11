namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

using Microsoft.AspNetCore.Components;

public partial class SeriesForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private ObservableCollection<string> Titles { get; set; } = new();
    private ObservableCollection<string> OriginalTitles { get; set; } = new();

    private TitlesForm<SeriesForm> TitlesForm { get; set; } = null!;
    private TitlesForm<SeriesForm> OriginalTitlesForm { get; set; } = null!;

    private SeriesWatchStatus WatchStatus { get; set; }
    private SeriesReleaseStatus ReleaseStatus { get; set; }

    private ListKindModel Kind { get; set; } = null!;

    private bool IsMiniseries { get; set; }

    private string ImdbId { get; set; } = String.Empty;
    private string RottenTomatoesLink { get; set; } = String.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.SetPropertyValues();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubscribeToAction<FetchSeriesResultAction>(_ => this.SetPropertyValues());
        }
    }

    private void FetchSeries()
    {
        if (this.ListItem != null)
        {
            this.Dispatcher.Dispatch(new FetchSeriesAction(this.ListItem.Id, this.State.Value.AvailableKinds));
        }
    }

    private void SetPropertyValues()
    {
        var series = this.State.Value.SeriesModel;

        this.Titles = series?.Titles?.Select(title => title.Name)?.ToObservableCollection()
            ?? new() { String.Empty };

        this.OriginalTitles = series?.OriginalTitles?.Select(title => title.Name)?.ToObservableCollection()
            ?? new() { String.Empty };

        this.WatchStatus = series?.WatchStatus ?? SeriesWatchStatus.NotWatched;
        this.ReleaseStatus = series?.ReleaseStatus ?? SeriesReleaseStatus.NotStarted;
        this.Kind = series?.Kind ?? this.State.Value.AvailableKinds.FirstOrDefault()!;
        this.IsMiniseries = series?.IsMiniseries ?? false;
        this.ImdbId = series?.ImdbId ?? String.Empty;
        this.RottenTomatoesLink = series?.RottenTomatoesLink ?? String.Empty;

        this.UpdateFormTitle();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.Titles.FirstOrDefault() ?? String.Empty;

    private void Save()
    { }

    private void Cancel() =>
        this.SetPropertyValues();
}
