using Cineaste.Client.Store.Forms.SeriesForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class SeriesMainForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public required override SeriesFormModel FormModel { get; set; }

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
            this.Dispatcher.Dispatch(new FetchSeriesAction(
                this.ListItem.Id, this.State.Value.AvailableKinds, this.State.Value.ListConfiguration));
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
        component.SequenceNumber != 1;

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

    private Task OnSave() =>
        this.WithValidation(this.Save.InvokeAsync);

    private async Task OnCancel()
    {
        this.ClearValidation();
        await this.Cancel.InvokeAsync();
    }

    private async Task Delete()
    {
        bool? delete = await this.DialogService.ShowMessageBox(
            title: this.Loc["SeriesForm.DeleteDialog.Title"],
            markupMessage: new MarkupString(this.Loc["SeriesForm.DeleteDialog.Body"]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new RemoveSeriesAction(id));
        }
    }

    private void GoToParentFranchise()
    {
        if (this.FormModel.ParentFranchiseId is Guid franchiseId)
        {
            this.Dispatcher.Dispatch(new GoToFranchiseAction(franchiseId));
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
