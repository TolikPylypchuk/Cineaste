namespace Cineaste.Client.Store.Forms.SeriesForm;

using Cineaste.Client.Store.ListPage;

public static class OpenSeriesFormReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceStartCreatingSeriesAction(
        SeriesFormState _,
        StartCreatingSeriesAction action) =>
        new() { Fetch = ApiCall.Success(), AvailableKinds = action.AvailableKinds };
}

[AutoConstructor]
public sealed partial class OpenSeriesFormEffect
{
    private readonly IState<SeriesFormState> state;

    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (state.Value.Model?.Id != action.Item.Id && action.Item.Type == ListItemType.Series)
        {
            dispatcher.Dispatch(new FetchSeriesAction(action.Item.Id, action.AvailableKinds));
        }

        return Task.CompletedTask;
    }
}
