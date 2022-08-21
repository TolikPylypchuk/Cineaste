namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeasonForm
{
    [Parameter]
    public SeasonModel? Season { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public EventCallback Delete { get; set; }

    [Parameter]
    public SeasonFormModel FormModel { get; set; } = null!;

    [Parameter]
    public EventCallback GoToNextComponent { get; set; }

    [Parameter]
    public EventCallback GoToPreviousComponent { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private ImmutableArray<SeasonWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeasonWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeasonReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeasonReleaseStatus>().ToImmutableArray();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.UpdateFormTitle();
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.Season);
        this.UpdateFormTitle();
    }

    private string RottenTomatoesLinkText(int part, bool multiple) =>
        multiple
            ? String.Format(this.Loc["Link.RottenTomatoesWithPartFormat"], part)
            : this.Loc["Link.RottenTomatoes"];

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void AddPeriod() =>
        this.FormModel.AddNewPeriod();

    private void OnPeriodRemoved(PeriodFormModel period) =>
        this.FormModel.RemovePeriod(period);

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void GoToSeries() =>
        this.Dispatcher.Dispatch(new GoToSeriesAction());

    private void Cancel() =>
        this.SetPropertyValues();
}
