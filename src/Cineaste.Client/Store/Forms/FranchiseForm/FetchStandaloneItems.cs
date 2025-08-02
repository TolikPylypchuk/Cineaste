namespace Cineaste.Client.Store.Forms.FranchiseForm;

public record FetchStandaloneItemsAction;

public record FetchStandaloneItemsResultAction(ApiResult<ImmutableList<ListItemModel>> Result)
    : ResultAction<ImmutableList<ListItemModel>>(Result);

public static class FetchStandaloneItemsReducers
{
    [ReducerMethod(typeof(FetchStandaloneItemsAction))]
    public static FranchiseFormState FetchStandaloneItemsAction(FranchiseFormState state) =>
        state with { FetchStandaloneItems = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceFetchStandaloneItemsResultAction(
        FranchiseFormState state,
        FetchStandaloneItemsResultAction action) =>
        action.Handle(
            onSuccess: items => state with { StandaloneItems = items, FetchStandaloneItems = ApiCall.Success() },
            onFailure: problem => state with { FetchStandaloneItems = ApiCall.Failure(problem) });
}

public class FetchStandaloneItemsEffects(IListApi api)
{
    [EffectMethod(typeof(FetchStandaloneItemsAction))]
    public async Task HandleFetchStandaloneItemsAction(IDispatcher dispatcher)
    {
        var result = await api.GetStandaloneListItems().ToApiResultAsync();
        dispatcher.Dispatch(new FetchStandaloneItemsResultAction(result));
    }
}
