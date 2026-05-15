using Cineaste.Client.Store.Forms;
using Cineaste.Client.Store.Forms.LimitedSeriesForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class LimitedSeriesForm
{
    [Parameter]
    public required ImmutableList<ListKindModel> AvailableKinds { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        [.. Enum.GetValues<SeriesWatchStatus>()];

    private ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        [.. Enum.GetValues<SeriesReleaseStatus>()];

    private bool IsSaving =>
        this.State.Value.Add.IsInProgress || this.State.Value.Update.IsInProgress;

    private object StatusErrorTrigger =>
        new { this.FormModel.WatchStatus, this.FormModel.ReleaseStatus };

    protected override void OnInitialized()
    {
        this.SubsribeToSuccessfulResult<FetchLimitedSeriesResultAction>(this.SetPropertyValues);
        this.SubsribeToSuccessfulResult<AddLimitedSeriesResultAction>(this.OnLimitedSeriesCreated);
        this.SubsribeToSuccessfulResult<UpdateLimitedSeriesResultAction>(this.OnLimitedSeriesUpdated);

        this.SubsribeToSuccessfulResult<SetLimitedSeriesPosterResultAction>(this.OnPosterUpdated);
        this.SubsribeToSuccessfulResult<RemoveLimitedSeriesPosterResultAction>(this.OnPosterUpdated);

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeFormModel();
    }

    protected override object? CreateSetPosterAction(Guid _, PosterRequest request) =>
        this.FormModel.BackingModel is { Id: var id } ? new SetLimitedSeriesPosterAction(id, request) : null;

    protected override object? CreateRemovePosterAction(Guid _) =>
        this.FormModel.BackingModel is { Id: var id } ? new RemoveLimitedSeriesPosterAction(id) : null;

    protected override void UpdateFormModel() =>
        this.FormModel.CopyFrom(this.State.Value.Model);

    private void InitializeFormModel()
    {
        if (this.FormModel is not null)
        {
            this.FormModel.TitlesUpdated -= this.OnTitlesUpdated;
            this.FormModel.OriginalTitlesUpdated -= this.OnOriginalTitlesUpdated;
        }

        this.FormModel = new(this.AvailableKinds.First(), this.State.Value.InitialParentFranchiseId);

        this.FormModel.CopyFrom(this.State.Value.Model);

        this.FormModel.TitlesUpdated += this.OnTitlesUpdated;
        this.FormModel.OriginalTitlesUpdated += this.OnOriginalTitlesUpdated;

        this.SetPropertyValues();
    }

    private void FetchLimitedSeries()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchLimitedSeriesAction(this.ListItem.Id));
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void OnTitlesUpdated(object? sender, EventArgs e) =>
        this.UpdateFormTitle();

    private void OnOriginalTitlesUpdated(object? sender, EventArgs e) =>
        this.StateHasChanged();

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private void OnPeriodChanged() =>
        this.StateHasChanged();

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private void Close() =>
        this.Dispatcher.Dispatch(new CloseItemAction());

    private void Save() =>
        this.WithValidation(request =>
            this.Dispatcher.Dispatch(this.ListItem is not null
                ? new UpdateLimitedSeriesAction(this.ListItem.Id, request)
                : new AddLimitedSeriesAction(request)));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();

        if (this.State.Value.InitialParentFranchiseId is Guid franchiseId)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(franchiseId));
        }
    }

    private async Task OpenPosterDialog()
    {
        if (this.FormModel.BackingModel is not null)
        {
            await this.OpenPosterDialog(this.FormTitle);
        }
    }

    private async Task RemovePoster() =>
        await this.RemovePoster(
            "LimitedSeriesForm.RemovePosterDialog.Title", "LimitedSeriesForm.RemovePosterDialog.Body");

    private async Task Remove()
    {
        bool? delete = await this.DialogService.ShowMessageBoxAsync(
            title: this.Loc["LimitedSeriesForm.RemoveDialog.Title"],
            markupMessage: new MarkupString(this.Loc["LimitedSeriesForm.RemoveDialog.Body"]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new RemoveLimitedSeriesAction(id));
        }
    }

    private void OnLimitedSeriesCreated() =>
        this.SetPropertyValues();

    private void OnLimitedSeriesUpdated() =>
        this.SetPropertyValues();

    private void StartAddingParentFranchise()
    {
        if (this.FormModel is
            {
                IsNew: false,
                HasChanges: false,
                ParentFranchiseId: null,
                BackingModel:
                {
                    Id: var id,
                    Titles: [var title, ..],
                    OriginalTitles: [var originalTitle, ..],
                    Period: { StartYear: int startYear, EndYear: int endYear },
                    ListItemColor: string listItemColor,
                    ListSequenceNumber: int listSequenceNumber
                }
            })
        {
            var action = new StartAddingParentFranchiseAction(
                title,
                originalTitle,
                new FranchiseItemModel(
                    id,
                    FranchiseItemType.LimitedSeries,
                    FirstSequenceNumber,
                    FirstSequenceNumber,
                    title.Name,
                    originalTitle.Name,
                    startYear,
                    endYear,
                    listItemColor,
                    listSequenceNumber),
                this.FormModel.Kind,
                FranchiseKindSource.LimitedSeries);

            this.Dispatcher.Dispatch(action);
        }
    }

    private void GoToParentFranchise()
    {
        if (this.FormModel.ParentFranchiseId is Guid franchiseId)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(franchiseId));
        }
    }

    private void GoToNextComponent()
    {
        if (this.FormModel.ParentFranchiseId is Guid franchiseId && this.FormModel.SequenceNumber is int num)
        {
            this.Dispatcher.Dispatch(new GoToFranchiseComponentAction(franchiseId, num + 1));
        }
    }

    private void GoToPreviousComponent()
    {
        if (this.FormModel.ParentFranchiseId is Guid franchiseId && this.FormModel.SequenceNumber is int num)
        {
            this.Dispatcher.Dispatch(new GoToFranchiseComponentAction(franchiseId, num - 1));
        }
    }
}
