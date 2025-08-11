using Cineaste.Client.Store.Forms.SeriesForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class SeasonForm
{
    [Parameter]
    public SeasonModel? Season { get; set; }

    [Parameter]
    public override SeasonFormModel FormModel { get; set; } = null!;

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

    private ImmutableList<string> PosterUrls { get; set; } = [];
    private int CurrentPosterUrlIndex { get; set; } = 0;

    private ImmutableArray<SeasonWatchStatus> AllWatchStatuses { get; } =
        [.. Enum.GetValues<SeasonWatchStatus>()];

    private ImmutableArray<SeasonReleaseStatus> AllReleaseStatuses { get; } =
        [.. Enum.GetValues<SeasonReleaseStatus>()];

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

        this.SubscribeToAction<SetSeasonPosterResultAction>(action =>
            action.OnSuccess(_ => this.OnPosterUpdated(action.PeriodId)));

        this.SubscribeToAction<RemoveSeasonPosterResultAction>(action =>
            action.OnSuccess(() => this.OnPosterUpdated(action.PeriodId)));

        this.UpdateFormTitle();

        this.PosterUrls = [.. this.FormModel.Periods.Select(p => p.BackingModel?.PosterUrl).WhereNotNull()];
        this.CurrentPosterUrlIndex = 0;
    }

    protected override object? CreateSetPosterAction(Guid periodId, PosterRequest request) =>
        this.FormModel.BackingModel is not null && this.State.Value.Model is { Id: var seriesId }
            ? new SetSeasonPosterAction(seriesId, periodId, request)
            : null;

    protected override object? CreateRemovePosterAction(Guid periodId) =>
        this.FormModel.BackingModel is not null && this.State.Value.Model is { Id: var seriesId }
            ? new RemoveSeasonPosterAction(seriesId, periodId)
            : null;

    protected override void UpdateFormModel()
    {
        if (this.Season is not null && this.State.Value.Model is { Seasons: var seasons })
        {
            this.FormModel.CopyFrom(seasons.FirstOrDefault(e => e.Id == this.Season.Id));
            this.PosterUrls = [.. this.FormModel.Periods.Select(p => p.BackingModel?.PosterUrl).WhereNotNull()];
        }
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

    private async Task OpenPosterDialog(Guid periodId)
    {
        if (this.FormModel.BackingModel is not null)
        {
            await this.OpenPosterDialog(this.FormTitle, periodId);
        }
    }

    private async Task RemovePoster(Guid periodId) =>
        await this.RemovePoster("SeasonForm.RemovePosterDialog.Title", "SeasonForm.RemovePosterDialog.Body", periodId);

    private Task OnGoToNextComponent() =>
        this.WithValidation(r => this.GoToNextComponent.InvokeAsync());

    private Task OnGoToPreviousComponent() =>
        this.WithValidation(r => this.GoToPreviousComponent.InvokeAsync());

    private void GoToSeries() =>
        this.WithValidation(r => this.Dispatcher.Dispatch(new GoToSeriesAction()));

    private void Close() =>
        this.Dispatcher.Dispatch(new CloseItemAction());

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }

    private void ShowNextPoster() =>
        this.CurrentPosterUrlIndex++;

    private void ShowPreviousPoster() =>
        this.CurrentPosterUrlIndex--;
}
