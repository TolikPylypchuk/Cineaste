namespace Cineaste.Client.Store.ListPage;

public sealed record FetchListAction;

public sealed record FetchListItemsAction(int Offset, int Size, CancellationToken Token);

public sealed record FetchListResultAction(ApiResult<ListModel> Result) : ResultAction<ListModel>(Result);

public sealed record FetchListItemsResultAction(
    ApiResult<OffsettableData<ListItemModel>> Result, CancellationToken Token)
    : ResultAction<OffsettableData<ListItemModel>>(Result);

public static class FetchListReducers
{
    [ReducerMethod(typeof(FetchListAction))]
    public static ListPageState ReduceFetchListsAction(ListPageState _) =>
        new() { IsLoading = true };

    [ReducerMethod]
    public static ListPageState ReduceFetchListResultAction(ListPageState state, FetchListResultAction action) =>
        action.Handle(
            onSuccess: list =>
                state with
                {
                    IsLoading = false,
                    IsLoaded = true,
                    Id = list.Id,
                    Items = [],
                    AvailableMovieKinds = list.MovieKinds,
                    AvailableSeriesKinds = list.SeriesKinds,
                    ListConfiguration = list.Config
                },
            onFailure: problem => new() { IsLoaded = true, Problem = problem });

    [ReducerMethod]
    public static ListPageState ReduceFetchListItemsResultAction(
        ListPageState state,
        FetchListItemsResultAction action) =>
        action.Handle(
            onSuccess: items =>
                state with
                {
                    Items = items.Items,
                    Offset = items.Metadata.Offset,
                    Size = items.Metadata.Size,
                    TotalItems = items.Metadata.TotalItems,
                },
            onFailure: problem => new() { IsLoaded = true, Problem = problem });
}

public sealed class FetchListEffects(IListApi api)
{
    [EffectMethod(typeof(FetchListAction))]
    public async Task HandleFetchListAction(IDispatcher dispatcher)
    {
        var result = await api.GetList().ToApiResultAsync();
        dispatcher.Dispatch(new FetchListResultAction(result));
    }

    [EffectMethod]
    public async Task HandleFetchListAction(FetchListItemsAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await api.GetListItems(action.Offset, action.Size, action.Token).ToApiResultAsync();
            dispatcher.Dispatch(new FetchListItemsResultAction(result, action.Token));
        } catch (TaskCanceledException)
        { }
    }
}
