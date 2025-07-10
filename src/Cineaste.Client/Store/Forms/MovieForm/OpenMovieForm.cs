using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.MovieForm;

public static class OpenMovieFormReducers
{
    [ReducerMethod]
    public static MovieFormState ReduceStartCreatingMovieAction(MovieFormState _, StartCreatingMovieAction action) =>
        new() { Fetch = ApiCall.Success(), AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static MovieFormState ReduceSelectItemAction(MovieFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Movie
            ? new() { Fetch = ApiCall.InProgress() }
            : state;
}

public sealed class OpenMovieFormEffect
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Movie)
        {
            dispatcher.Dispatch(new FetchMovieAction(action.Item.Id, action.AvailableKinds));
        }

        return Task.CompletedTask;
    }
}
