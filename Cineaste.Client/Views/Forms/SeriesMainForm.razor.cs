namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeriesMainForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public SeriesFormModel FormModel { get; set; } = null!;

    [Parameter]
    public string FormTitle { get; set; } = String.Empty;

    [Parameter]
    public EventCallback FormTitleUpdated { get; set; }

    [Parameter]
    public EventCallback Save { get; set; }

    [Parameter]
    public EventCallback Cancel { get; set; }

    private ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();

    private void FetchSeries()
    {
        if (this.ListItem != null)
        {
            this.Dispatcher.Dispatch(new FetchSeriesAction(
                this.ListItem.Id, this.State.Value.AvailableKinds, this.State.Value.ListConfiguration));
        }
    }

    private void AddTitle(ObservableCollection<string> titles) =>
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

    private async Task UpdateFormTitle() =>
        await this.FormTitleUpdated.InvokeAsync();

    private void OpenSeriesComponentForm(ISeriesComponentFormModel component) =>
        this.Dispatcher.Dispatch(new OpenSeriesComponentFormAction(component));

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private async Task Delete()
    {
        bool? delete = await this.DialogService.Confirm(
            this.Loc["SeriesForm.DeleteDialog.Body"],
            this.Loc["SeriesForm.DeleteDialog.Title"],
            new()
            {
                OkButtonText = this.Loc["Confirmation.Confirm"],
                CancelButtonText = this.Loc["Confirmation.Cancel"],
                CloseDialogOnEsc = true,
                CloseDialogOnOverlayClick = true
            });

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteSeriesAction(id));
        }
    }
}
