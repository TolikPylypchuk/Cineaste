using Cineaste.Client.Store.Forms;
using Cineaste.Client.Store.Forms.FranchiseForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class FranchiseForm
{
    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public required ImmutableList<ListKindModel> AvailableMovieKinds { get; set; }

    [Parameter]
    public required ImmutableList<ListKindModel> AvailableSeriesKinds { get; set; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    private string FormTitle { get; set; } = String.Empty;

    public required MudDataGrid<FranchiseFormComponent> ComponentGrid { get; set; }

    private bool IsSaving =>
        this.State.Value.Add.IsInProgress || this.State.Value.Update.IsInProgress;

    protected override void OnInitialized()
    {
        this.SubsribeToSuccessfulResult<FetchFranchiseResultAction>(this.SetPropertyValues);
        this.SubsribeToSuccessfulResult<AddFranchiseResultAction>(this.OnFranchiseCreated);
        this.SubsribeToSuccessfulResult<UpdateFranchiseResultAction>(this.OnFranchiseUpdated);

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeFormModel();
    }

    private void InitializeFormModel()
    {
        if (this.FormModel is not null)
        {
            this.FormModel.TitlesUpdated -= this.OnTitlesUpdated;
            this.FormModel.OriginalTitlesUpdated -= this.OnOriginalTitlesUpdated;
        }

        this.FormModel = new(
            this.AvailableMovieKinds.First(),
            FranchiseKindSource.Movie,
            this.State.Value.InitialParentFranchiseId);

        this.FormModel.CopyFrom(this.State.Value.Model);

        this.FormModel.TitlesUpdated += this.OnTitlesUpdated;
        this.FormModel.OriginalTitlesUpdated += this.OnOriginalTitlesUpdated;

        this.SetPropertyValues();
    }

    private void FetchFranchise()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchFranchiseAction(this.ListItem.Id));
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

    private void OnKindSourceValueChanged(FranchiseKindSource newValue)
    {
        this.FormModel.KindSource = newValue;

        switch (newValue)
        {
            case FranchiseKindSource.Movie:
                this.FormModel.Kind = this.AvailableMovieKinds[0];
                break;
            case FranchiseKindSource.Series:
                this.FormModel.Kind = this.AvailableSeriesKinds[0];
                break;
        }
    }

    private bool CanMoveUp(FranchiseFormComponent component) =>
        component.SequenceNumber != FirstSequenceNumber;

    private void MoveUp(FranchiseFormComponent component) =>
        this.FormModel.MoveComponentUp(component);

    private bool CanMoveDown(FranchiseFormComponent component) =>
        component.SequenceNumber != this.FormModel.Components.Count;

    private void MoveDown(FranchiseFormComponent component) =>
        this.FormModel.MoveComponentDown(component);

    private void Detach(FranchiseFormComponent component) =>
        this.FormModel.DetachComponent(component);

    private void OpenComponentForm(FranchiseFormComponent component)
    {
        if (component is not null && !this.FormModel.HasChanges)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(component.Id));
        }
    }

    private void Close() =>
        this.Dispatcher.Dispatch(new CloseItemAction());

    private void Save() =>
        this.WithValidation(request =>
            this.Dispatcher.Dispatch(this.ListItem is not null
                ? new UpdateFranchiseAction(this.ListItem.Id, request)
                : new AddFranchiseAction(request)));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();

        if (this.State.Value.InitialItemId is Guid itemId)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(itemId));
        } else if (this.State.Value.InitialParentFranchiseId is Guid franchiseId)
        {
            this.Dispatcher.Dispatch(new GoToListItemAction(franchiseId));
        }
    }

    private async Task Remove()
    {
        bool? delete = await this.DialogService.ShowMessageBox(
            title: this.Loc["FranchiseForm.RemoveDialog.Title"],
            markupMessage: new MarkupString(this.Loc["FranchiseForm.RemoveDialog.Body"]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new RemoveFranchiseAction(id));
        }
    }

    private void OnFranchiseCreated() =>
        this.SetPropertyValues();

    private void OnFranchiseUpdated() =>
        this.SetPropertyValues();

    private void StartAddingMovie() =>
        this.StartAddingComponent(id => new StartAddingMovieAction(id));

    private void StartAddingSeries() =>
        this.StartAddingComponent(id => new StartAddingSeriesAction(id));

    private void StartAddingFranchise() =>
        this.StartAddingComponent(id => new StartAddingFranchiseAction(id));

    private void StartAddingComponent<T>(Func<Guid, T> action)
    {
        if (this.FormModel is { IsNew: false, HasChanges: false, BackingModel.Id: var id })
        {
            this.Dispatcher.Dispatch(action(id));
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
                    OriginalTitles: [var originalTitle, ..]
                }
            })
        {
            var startYear = this.FormModel.Components.Select(component => component.StartYear).Min() ?? 0;
            var endYear = this.FormModel.Components.Select(component => component.EndYear).Max() ?? 0;

            var action = new StartAddingParentFranchiseAction(
                title,
                originalTitle,
                new FranchiseItemModel(
                    id,
                    FirstSequenceNumber,
                    FirstSequenceNumber,
                    title.Name,
                    startYear,
                    endYear,
                    FranchiseItemType.Franchise),
                this.FormModel.Kind,
                this.FormModel.KindSource);

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
