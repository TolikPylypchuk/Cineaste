using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.FranchiseForm;

public static class OpenFranchiseFormReducers
{
    [ReducerMethod]
    public static FranchiseFormState ReduceStartCreatingFranchiseAction(
        FranchiseFormState _,
        StartCreatingFranchiseAction action) =>
        new() { Fetch = ApiCall.Success(), AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static FranchiseFormState ReduceSelectItemAction(FranchiseFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Franchise
            ? new() { Fetch = ApiCall.InProgress() }
            : state;
}

public sealed class OpenFranchiseFormEffect
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Franchise)
        {
            dispatcher.Dispatch(new FetchFranchiseAction(action.Item.Id, action.AvailableKinds));
        }

        return Task.CompletedTask;
    }
}
