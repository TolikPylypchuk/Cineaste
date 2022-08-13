namespace Cineaste.Client.Store.Forms.SeriesForm;

using Cineaste.Client.Store.ListPage;

public static class OpenSeriesFormReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceStartCreatingSeriesAction(
        SeriesFormState _,
        StartCreatingSeriesAction action) =>
        new()
        {
            Fetch = ApiCall.Success(),
            AvailableKinds = action.AvailableKinds,
            ListConfiguration = action.ListConfiguration
        };

    [ReducerMethod]
    public static SeriesFormState ReduceSelectItemAction(SeriesFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Series
            ? new() { Fetch = ApiCall.InProgress() }
            : state;
}

public sealed class OpenSeriesFormEffect
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Series)
        {
            dispatcher.Dispatch(new FetchSeriesAction(action.Item.Id, action.AvailableKinds, action.ListConfiguration));
        }

        return Task.CompletedTask;
    }
}
