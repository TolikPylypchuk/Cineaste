namespace Cineaste.Client.Store.ListPage;

public sealed record GoToListItemAction(Guid Id);

public sealed record GoToFranchiseComponentAction(Guid FranchiseId, int SequenceNumber);

public sealed record GoToListItemResultAction(ApiResult<ListItemModel> Result) : ResultAction<ListItemModel>(Result);

public static class GoToListItemReducers
{
    [ReducerMethod]
    public static ListPageState ReduceGoToListItemResultAction(ListPageState state, GoToListItemResultAction action) =>
        action.Handle(
            onSuccess: result =>
                state with { SelectedItem = result, SelectionMode = result.Type.ToListPageSelectionMode() },
            onFailure: _ => state);
}

public sealed class GoToListItemEffects(IListApi api)
{
    [EffectMethod]
    public async Task HandleGoToListItemAction(GoToListItemAction action, IDispatcher dispatcher)
    {
        var result = await api.GetListItem(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new GoToListItemResultAction(result));
    }

    [EffectMethod]
    public async Task HandleGoToFranchiseComponentAction(GoToFranchiseComponentAction action, IDispatcher dispatcher)
    {
        var result = await api.GetListItemByParentFranchise(action.FranchiseId, action.SequenceNumber)
            .ToApiResultAsync();

        dispatcher.Dispatch(new GoToListItemResultAction(result));
    }
}
