using Cineaste.Client.Store.Forms.FranchiseForm;

namespace Cineaste.Client.Components.Forms;

public partial class FranchiseForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    private string FormTitle { get; set; } = String.Empty;

    private bool IsSaving =>
        this.State.Value.Create.IsInProgress || this.State.Value.Update.IsInProgress;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel = new(this.ListId);

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

        this.SetPropertyValues();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubsribeToSuccessfulResult<FetchFranchiseResultAction>(this.SetPropertyValues);
            this.SubsribeToSuccessfulResult<CreateFranchiseResultAction>(this.OnFranchiseCreated);
            this.SubsribeToSuccessfulResult<UpdateFranchiseResultAction>(this.OnFranchiseUpdated);
        }
    }

    private void FetchFranchise()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchFranchiseAction(this.ListItem.Id, this.State.Value.AvailableKinds));
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private void Save() =>
        this.WithValidation(request =>
            this.Dispatcher.Dispatch(this.ListItem is not null
                ? new UpdateFranchiseAction(this.ListItem.Id, request)
                : new CreateFranchiseAction(request)));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }

    private async Task Delete()
    {
        bool? delete = await this.DialogService.ShowMessageBox(
            title: this.Loc["FranchiseForm.DeleteDialog.Title"],
            markupMessage: new MarkupString(this.Loc["FranchiseForm.DeleteDialog.Body"]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteFranchiseAction(id));
        }
    }

    private void OnFranchiseCreated() =>
        this.SetPropertyValues();

    private void OnFranchiseUpdated() =>
        this.SetPropertyValues();
}
