using System.Collections.Concurrent;

using Cineaste.Client.Store;
using Cineaste.Client.Store.Forms.FranchiseForm;
using Cineaste.Client.Store.Forms.MovieForm;
using Cineaste.Client.Store.Forms.SeriesForm;
using Cineaste.Client.Store.ListPage;

using ListItemModelSource = System.Threading.Tasks.TaskCompletionSource<
    Cineaste.Shared.Models.OffsettableData<Cineaste.Shared.Models.List.ListItemModel>?>;

namespace Cineaste.Client.Components.List;

public partial class ListComponent
{
    private readonly int ItemHeight = 64;

    private readonly ConcurrentDictionary<CancellationToken, ListItemModelSource> dataSources = [];

    [Inject]
    public required IScrollManager ScrollManager { get; set; }

    public required MudDataGrid<ListItemModel> DataGridRef { get; set; }

    protected override void OnInitialized()
    {
        this.SubscribeToAction<CloseItemAction>(async _ =>
        {
            await this.DataGridRef.SetSelectedItemAsync(null!);
            this.StateHasChanged();
        });

        this.SubscribeToAction<FetchListItemsResultAction>(result => result.Handle(
            onSuccess: data => this.TrySetResult(data, result.Token),
            onFailure: _ => this.TrySetResult(null, result.Token)));

        this.SubsribeToSuccessfulResult<AddMovieResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<AddSeriesResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<AddFranchiseResultAction>(this.ReloadList);

        this.SubsribeToSuccessfulResult<UpdateMovieResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<UpdateSeriesResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<UpdateFranchiseResultAction>(this.ReloadList);

        this.SubsribeToSuccessfulResult<RemoveMovieResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<RemoveSeriesResultAction>(this.ReloadList);
        this.SubsribeToSuccessfulResult<RemoveFranchiseResultAction>(this.ReloadList);

        this.SubsribeToSuccessfulResult<GoToListItemResultAction>(this.ReloadList);

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!this.State.Value.IsLoaded)
        {
            this.FetchList();
        }
    }

    private async Task<GridData<ListItemModel>> ServerDataFunc(
        GridStateVirtualize<ListItemModel> gridState,
        CancellationToken token)
    {
        var dataSource = new ListItemModelSource();
        this.dataSources.TryAdd(token, dataSource);

        token.Register(() => this.TrySetResult(null, token));

        this.Dispatcher.Dispatch(new FetchListItemsAction(gridState.StartIndex, gridState.Count, token));

        var data = await dataSource.Task;

        return data is not null
            ? new() { Items = data.Items, TotalItems = data.Metadata.TotalItems }
            : new() { Items = [], TotalItems = 0 };
    }

    private void FetchList() =>
        this.Dispatcher.Dispatch(new FetchListAction());

    private void SelectItem(ListItemModel? item)
    {
        if (item is not null && item != this.State.Value.SelectedItem)
        {
            this.Dispatcher.Dispatch(new SelectItemAction(item));
        }
    }

    private void TrySetResult(OffsettableData<ListItemModel>? data, CancellationToken token)
    {
        if (this.dataSources.TryRemove(token, out var dataSource))
        {
            dataSource.TrySetResult(data);
        }
    }

    private async void ReloadList()
    {
        await this.DataGridRef.ReloadServerData();
        this.StateHasChanged();

        if (this.State.Value.SelectedItem is { ListSequenceNumber: int sequenceNumber })
        {
            await this.ScrollToListItem(sequenceNumber);
        }
    }

    private async ValueTask ScrollToListItem(int sequenceNumber)
    {
        if (sequenceNumber == 0)
        {
            return;
        }

        var itemIndex = sequenceNumber - 1;

        await this.ScrollManager.ScrollToAsync(
            $"#{this.DataGridRef.FieldId} .mud-table-container",
            0,
            Math.Max(0, this.ItemHeight * (itemIndex - 4)), // Subtract 4 so that the item is not at the very top
            ScrollBehavior.Auto);

        this.StateHasChanged();
    }
}
