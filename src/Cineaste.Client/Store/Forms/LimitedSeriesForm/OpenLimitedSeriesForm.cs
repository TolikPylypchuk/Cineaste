using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public static class OpenLimitedSeriesFormReducers
{
    [ReducerMethod]
    public static LimitedSeriesFormState ReduceStartAddingLimitedSeriesAction(
        LimitedSeriesFormState _,
        StartAddingLimitedSeriesAction action) =>
        new() { InitialParentFranchiseId = action.ParentFranchiseId, Fetch = ApiCall.Success() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceSelectItemAction(
        LimitedSeriesFormState state,
        SelectItemAction action) =>
        action.Item.Type == ListItemType.LimitedSeries
            ? new() { Fetch = ApiCall.InProgress() }
            : state;

    [ReducerMethod(typeof(GoToListItemAction))]
    public static LimitedSeriesFormState ReduceGoToListItemAction(LimitedSeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod(typeof(GoToFranchiseComponentAction))]
    public static LimitedSeriesFormState ReduceGoToFranchiseComponentAction(LimitedSeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };
}

public sealed class OpenLimitedSeriesFormEffects
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.LimitedSeries)
        {
            dispatcher.Dispatch(new FetchLimitedSeriesAction(action.Item.Id));
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGoToListItemResult(GoToListItemResultAction action, IDispatcher dispatcher)
    {
        action.Handle(
            onSuccess: item =>
            {
                if (item.Type == ListItemType.LimitedSeries)
                {
                    dispatcher.Dispatch(new FetchLimitedSeriesAction(item.Id));
                }
            },
            onFailure: _ => { });

        return Task.CompletedTask;
    }
}
