namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeasonForm
{
    [Parameter]
    public SeasonModel? Season { get; set; }

    [Parameter]
    public override SeasonFormModel FormModel { get; set; } = null!;

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public EventCallback Delete { get; set; }

    [Parameter]
    public EventCallback GoToNextComponent { get; set; }

    [Parameter]
    public EventCallback GoToPreviousComponent { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private object StatusErrorTrigger =>
        new { this.FormModel.WatchStatus, this.FormModel.ReleaseStatus };

    private object PeriodValidationTrigger { get; set; } = new();

    private ImmutableArray<SeasonWatchStatus> AllWatchStatuses { get; } =
        [.. Enum.GetValues<SeasonWatchStatus>()];

    private ImmutableArray<SeasonReleaseStatus> AllReleaseStatuses { get; } =
        [.. Enum.GetValues<SeasonReleaseStatus>()];

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

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

    private void AddPeriod() =>
        this.FormModel.AddNewPeriod();

    private void OnPeriodRemoved(PeriodFormModel period) =>
        this.FormModel.RemovePeriod(period);

    private void OnPeriodChanged()
    {
        this.PeriodValidationTrigger = this.FormModel.Periods.Aggregate(
            String.Empty,
            (acc, period) => $"{acc}{period.StartMonth}{period.StartYear}{period.EndMonth}{period.EndYear}");

        this.StateHasChanged();
    }

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private Task OnGoToNextComponent() =>
        this.WithValidation(r => this.GoToNextComponent.InvokeAsync());

    private Task OnGoToPreviousComponent() =>
        this.WithValidation(r => this.GoToPreviousComponent.InvokeAsync());

    private void GoToSeries() =>
        this.WithValidation(r => this.Dispatcher.Dispatch(new GoToSeriesAction()));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }
}
