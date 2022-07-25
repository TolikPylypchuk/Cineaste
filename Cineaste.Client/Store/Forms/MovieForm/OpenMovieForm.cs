namespace Cineaste.Client.Store.Forms.MovieForm;

using Cineaste.Client.Store.ListPage;

public static class OpenMovieFormReducers
{
    [ReducerMethod]
    public static MovieFormState ReduceStartCreatingMovieAction(MovieFormState _, StartCreatingMovieAction action) =>
        new() { Fetch = ApiCall.Success(), AvailableKinds = action.AvailableKinds };
}

[AutoConstructor]
public sealed partial class OpenMovieFormEffect
{
    private readonly IState<MovieFormState> state;

    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (state.Value.Model?.Id != action.Item.Id && action.Item.Type == ListItemType.Movie)
        {
            dispatcher.Dispatch(new FetchMovieAction(action.Item.Id, action.AvailableKinds));
        }

        return Task.CompletedTask;
    }
}
