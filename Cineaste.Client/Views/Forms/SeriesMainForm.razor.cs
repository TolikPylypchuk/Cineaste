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

    private string FormTitle { get; set; } = String.Empty;

    private SeriesFormModel FormModel { get; set; } = null!;

    private ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel = new(this.State.Value.AvailableKinds);
        this.SetPropertyValues();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubsribeToSuccessfulResult<FetchSeriesResultAction>(this.SetPropertyValues);
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
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void OpenSeriesComponentForm(ISeriesComponentFormModel component) =>
        this.Dispatcher.Dispatch(new OpenSeriesComponentFormAction(component));

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private void Save()
    { }

    private void Cancel() =>
        this.SetPropertyValues();

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
