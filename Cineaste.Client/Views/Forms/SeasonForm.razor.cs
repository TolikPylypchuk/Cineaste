namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeasonForm
{
    [Parameter]
    public SeasonModel? Season { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public SeasonFormModel FormModel { get; set; } = null!;

    private string FormTitle { get; set; } = String.Empty;

    private ImmutableArray<SeasonWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeasonWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeasonReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeasonReleaseStatus>().ToImmutableArray();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.SetPropertyValues();
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.Season);
        this.UpdateFormTitle();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void GoToSeries() =>
        this.Dispatcher.Dispatch(new GoToSeriesAction());

    private void Cancel() =>
        this.SetPropertyValues();

    private void Delete() =>
        this.Dispatcher.Dispatch(new GoToSeriesAction());
}
