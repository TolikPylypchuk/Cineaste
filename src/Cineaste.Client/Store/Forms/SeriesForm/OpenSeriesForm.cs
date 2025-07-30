using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.SeriesForm;

public static class OpenSeriesFormReducers
{
    [ReducerMethod(typeof(StartAddingSeriesAction))]
    public static SeriesFormState ReduceStartAddingSeriesAction(SeriesFormState _) =>
        new() { Fetch = ApiCall.Success() };

    [ReducerMethod]
    public static SeriesFormState ReduceSelectItemAction(SeriesFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Series
            ? new() { Fetch = ApiCall.InProgress() }
            : state;

    [ReducerMethod(typeof(GoToListItemAction))]
    public static SeriesFormState ReduceGoToListItemAction(SeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod(typeof(GoToFranchiseComponentAction))]
    public static SeriesFormState ReduceGoToFranchiseComponentAction(SeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };
}

public sealed class OpenSeriesFormEffects
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Series)
        {
            dispatcher.Dispatch(new FetchSeriesAction(action.Item.Id));
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGoToListItemResult(GoToListItemResultAction action, IDispatcher dispatcher)
    {
        action.Handle(
            onSuccess: item =>
            {
                if (item.Type == ListItemType.Series)
                {
                    dispatcher.Dispatch(new FetchSeriesAction(item.Id));
                }
            },
            onFailure: _ => { });

        return Task.CompletedTask;
    }
}
