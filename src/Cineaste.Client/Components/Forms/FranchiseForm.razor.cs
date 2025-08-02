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

    public required MudAutocomplete<ListItemModel> StandaloneItemSelect { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    public required MudDataGrid<FranchiseFormComponent> ComponentGrid { get; set; }

    private SortedSet<ListItemModel> StandaloneItems { get; } = [];
    private Dictionary<Guid, ListItemModel> AddedStandaloneItems { get; } = [];

    private ListItemModel? SelectedStandaloneItem { get; set; }

    private bool IsSaving =>
        this.State.Value.Add.IsInProgress || this.State.Value.Update.IsInProgress;

    protected override void OnInitialized()
    {
        this.SubsribeToSuccessfulResult<FetchFranchiseResultAction>(this.SetPropertyValues);
        this.SubsribeToSuccessfulResult<AddFranchiseResultAction>(this.OnFranchiseCreated);
        this.SubsribeToSuccessfulResult<UpdateFranchiseResultAction>(this.OnFranchiseUpdated);
        this.SubsribeToSuccessfulResult<FetchStandaloneItemsResultAction>(this.UpdateStandaloneItems);

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

    private void FetchStandaloneItems() =>
        this.Dispatcher.Dispatch(new FetchStandaloneItemsAction());

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

    private Task<IEnumerable<ListItemModel>> SearchStandaloneItems(string value, CancellationToken token)
    {
        if (String.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult<IEnumerable<ListItemModel>>(this.StandaloneItems);
        }

        var trimmedValue = value.Trim();

        var result = this.StandaloneItems
            .Where(item => item.Title.Contains(trimmedValue, StringComparison.InvariantCultureIgnoreCase) ||
                item.OriginalTitle.Contains(trimmedValue, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(result);
    }

    private void UpdateStandaloneItems()
    {
        var itemIdToSkip = this.State.Value.Model?.FranchiseItem?.RootFranchiseId ?? this.State.Value.Model?.Id;

        this.StandaloneItems.Clear();

        foreach (var item in this.State.Value.StandaloneItems.Where(item => item.Id != itemIdToSkip))
        {
            this.StandaloneItems.Add(item);
        }
    }

    private async Task OnSelectedStandaloneItemChanged(ListItemModel? item)
    {
        if (item is null)
        {
            return;
        }

        this.FormModel.AttachNewComponent(item);

        this.StandaloneItems.Remove(item);
        this.AddedStandaloneItems.Add(item.Id, item);

        await this.StandaloneItemSelect.ClearAsync();
    }

    private bool CanMoveUp(FranchiseFormComponent component) =>
        component.SequenceNumber != FirstSequenceNumber;

    private void MoveUp(FranchiseFormComponent component) =>
        this.FormModel.MoveComponentUp(component);

    private bool CanMoveDown(FranchiseFormComponent component) =>
        component.SequenceNumber != this.FormModel.Components.Count;

    private void MoveDown(FranchiseFormComponent component) =>
        this.FormModel.MoveComponentDown(component);

    private void Detach(FranchiseFormComponent component)
    {
        this.FormModel.DetachComponent(component);

        if (this.AddedStandaloneItems.TryGetValue(component.Id, out var item))
        {
            this.StandaloneItems.Add(item);
            this.AddedStandaloneItems.Remove(component.Id);
        } else if (this.State.Value.Model is { } model &&
            model.Items.FirstOrDefault(item => item.Id == component.Id) is { } franchiseItem)
        {
            var listFranchiseItem = new ListFranchiseItemModel(
                model.Id, franchiseItem.SequenceNumber, franchiseItem.DisplayNumber, model.IsLooselyConnected);

            this.StandaloneItems.Add(new ListItemModel(
                franchiseItem.Id,
                franchiseItem.Type.ToListItemType(),
                franchiseItem.Title,
                franchiseItem.OriginalTitle,
                franchiseItem.StartYear,
                franchiseItem.EndYear,
                franchiseItem.ListItemColor,
                franchiseItem.ListSequenceNumber,
                listFranchiseItem));
        }
    }

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

        foreach (var item in this.AddedStandaloneItems.Values)
        {
            this.StandaloneItems.Add(item);
        }

        this.AddedStandaloneItems.Clear();

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
                    OriginalTitles: [var originalTitle, ..],
                    ListItemColor: string listItemColor,
                    ListSequenceNumber: int listSequenceNumber
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
                    FranchiseItemType.Franchise,
                    FirstSequenceNumber,
                    FirstSequenceNumber,
                    title.Name,
                    originalTitle.Name,
                    startYear,
                    endYear,
                    listItemColor,
                    listSequenceNumber),
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
