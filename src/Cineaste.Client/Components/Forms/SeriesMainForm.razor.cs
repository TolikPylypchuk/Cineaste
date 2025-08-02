using Cineaste.Client.Store.Forms;
using Cineaste.Client.Store.Forms.SeriesForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class SeriesMainForm
{
    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public required override SeriesFormModel FormModel { get; set; }

    [Parameter]
    public required ListConfigurationModel ListConfiguration { get; set; }

    [Parameter]
    public required ImmutableList<ListKindModel> AvailableKinds { get; set; }

    [Parameter]
    public string FormTitle { get; set; } = String.Empty;

    [Parameter]
    public EventCallback FormTitleUpdated { get; set; }

    [Parameter]
    public EventCallback<SeriesRequest> Save { get; set; }

    [Parameter]
    public EventCallback Cancel { get; set; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    public required MudDataGrid<ISeriesComponentFormModel> ComponentGrid { get; set; }

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
        this.SubscribeToAction<GoToSeriesAction>(async _ =>
        {
            await this.ComponentGrid.SetSelectedItemAsync(null!);
            this.StateHasChanged();
        });

        base.OnInitialized();
    }

    private void FetchSeries()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchSeriesAction(this.ListItem.Id));
        }
    }

    private void AddSeason()
    {
        var season = this.FormModel.AddSeason();
        this.OpenSeriesComponentForm(season);
    }

    private void AddSpecialEpisode()
    {
        var episode = this.FormModel.AddSpecialEpisode();
        this.OpenSeriesComponentForm(episode);
    }

    private bool CanMoveUp(ISeriesComponentFormModel component) =>
        component.SequenceNumber != FirstSequenceNumber;

    private void MoveUp(ISeriesComponentFormModel component) =>
        this.FormModel.MoveComponentUp(component);

    private bool CanMoveDown(ISeriesComponentFormModel component) =>
        component.SequenceNumber != this.FormModel.Components.Count;

    private void MoveDown(ISeriesComponentFormModel component) =>
        this.FormModel.MoveComponentDown(component);

    private void OpenSeriesComponentForm(ISeriesComponentFormModel component) =>
        this.Dispatcher.Dispatch(new OpenSeriesComponentFormAction(component));

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private void Close() =>
        this.Dispatcher.Dispatch(new CloseItemAction());

    private Task OnSave() =>
        this.WithValidation(this.Save.InvokeAsync);

    private async Task OnCancel()
    {
        this.ClearValidation();
        await this.Cancel.InvokeAsync();

        if (this.State.Value.InitialParentFranchiseId is Guid franchiseId)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(franchiseId));
        }
    }

    private async Task Remove()
    {
        bool? delete = await this.DialogService.ShowMessageBox(
            title: this.Loc["SeriesForm.RemoveDialog.Title"],
            markupMessage: new MarkupString(this.Loc["SeriesForm.RemoveDialog.Body"]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new RemoveSeriesAction(id));
        }
    }

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
                    StartYear: int startYear,
                    EndYear: int endYear,
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
                    FranchiseItemType.Series,
                    FirstSequenceNumber,
                    FirstSequenceNumber,
                    title.Name,
                    originalTitle.Name,
                    startYear,
                    endYear,
                    listItemColor,
                    listSequenceNumber),
                this.FormModel.Kind,
                FranchiseKindSource.Series);

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
