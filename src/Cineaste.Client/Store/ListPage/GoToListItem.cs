using Cineaste.Client.Store.Forms.MovieForm;

namespace Cineaste.Client.Store.ListPage;

public sealed record GoToFranchiseAction(Guid FranchiseId);

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
    public async Task HandleGoToFranchiseAction(GoToFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.GetListItem(action.FranchiseId).ToApiResultAsync();
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
