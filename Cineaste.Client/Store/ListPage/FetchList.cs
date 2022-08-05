namespace Cineaste.Client.Store.ListPage;

public sealed record FetchListAction(string Handle);

public sealed record FetchListResultAction(ApiResult<ListModel> Result) : ResultAction<ListModel>(Result);

public static class FetchListReducers
{
    [ReducerMethod(typeof(FetchListAction))]
    public static ListPageState ReduceFetchListsAction(ListPageState _) =>
        new() { IsLoading = true };

    [ReducerMethod]
    public static ListPageState ReduceFetchListsResultAction(ListPageState state, FetchListResultAction action) =>
        action.Handle(
            onSuccess: list =>
                state with
                {
                    IsLoading = false,
                    IsLoaded = true,
                    Id = list.Id,
                    Name = list.Name,
                    Container = ListItemContainer.Create(list),
                    AvailableMovieKinds = list.MovieKinds,
                    AvailableSeriesKinds = list.SeriesKinds,
                    ListConfiguration = list.Config
                },
            onFailure: problem => new() { IsLoaded = true, Problem = problem });
}

[AutoConstructor]
public sealed partial class FetchListEffect
{
    private readonly IListApi api;

    [EffectMethod]
    public async Task HandleFetchListAction(FetchListAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetList(action.Handle).ToApiResultAsync();
        dispatcher.Dispatch(new FetchListResultAction(result));
    }
}
