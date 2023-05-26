namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeriesMainForm
{
    private ConfirmationDialog? deleteConfirmationDialog;

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

    private ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();

    private bool IsSaving =>
        this.State.Value.Create.IsInProgress || this.State.Value.Update.IsInProgress;

    private object StatusErrorTrigger =>
        new { this.FormModel.WatchStatus, this.FormModel.ReleaseStatus };

    private ISeriesComponentFormModel? ComponentWithMenu { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.GlobalEventProvider.GlobalMouseClick += (sender, e) =>
        {
            this.HideMenu();
            this.StateHasChanged();
        };

        this.GlobalEventProvider.GlobalKeyReleased += (sender, e) =>
        {
            if (e.Key == "Escape")
            {
                this.HideMenu();
                this.StateHasChanged();
            }
        };
    }

    private void FetchSeries()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchSeriesAction(
                this.ListItem.Id, this.State.Value.AvailableKinds, this.State.Value.ListConfiguration));
        }
    }

    private void AddTitle(ICollection<string> titles) =>
        titles.Add(String.Empty);

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

    private void ShowMenu(ISeriesComponentFormModel component) =>
        this.ComponentWithMenu = component;

    private void HideMenu() =>
        this.ComponentWithMenu = null;

    private bool ShouldShowMenu(ISeriesComponentFormModel component) =>
        this.ComponentWithMenu == component;

    private bool ShouldShowAnyMenu() =>
        this.ComponentWithMenu is not null;

    private bool CanMoveUp(ISeriesComponentFormModel component) =>
        component.SequenceNumber != 1;

    private void MoveUp(ISeriesComponentFormModel component)
    {
        this.HideMenu();
        this.FormModel.MoveComponentUp(component);
    }

    private bool CanMoveDown(ISeriesComponentFormModel component) =>
        component.SequenceNumber != this.FormModel.Components.Count;

    private void MoveDown(ISeriesComponentFormModel component)
    {
        this.HideMenu();
        this.FormModel.MoveComponentDown(component);
    }

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
        if (this.deleteConfirmationDialog is null)
        {
            return;
        }

        bool? delete = await this.deleteConfirmationDialog.RequestConfirmation();

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteSeriesAction(id));
        }
    }
}
