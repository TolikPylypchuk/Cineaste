namespace Cineaste.Client.Store.HomePage;

public sealed record FetchListsAction;

public sealed record FetchListsResultAction(ApiResult<List<SimpleListModel>> Result)
    : ResultAction<List<SimpleListModel>>(Result);

public static class FetchListsReducers
{
    [ReducerMethod(typeof(FetchListsAction))]
    public static HomePageState ReduceFetchListsAction(HomePageState _) =>
        new() { IsLoading = true };

    [ReducerMethod]
    public static HomePageState ReduceFetchListsResultAction(HomePageState state, FetchListsResultAction action) =>
        action.Handle(
            onSuccess: lists => state with { IsLoading = false, Lists = lists.ToImmutableList() },
            onFailure: problem => state with { IsLoading = false, Problem = problem });
}

[AutoConstructor]
public sealed partial class FetchListsEffect
{
    private readonly IListApi api;

    [EffectMethod(typeof(FetchListsAction))]
    public async Task HandleFetchListsAction(IDispatcher dispatcher)
    {
        var result = await this.api.GetLists().ToApiResultAsync();
        dispatcher.Dispatch(new FetchListsResultAction(result));
    }
}
