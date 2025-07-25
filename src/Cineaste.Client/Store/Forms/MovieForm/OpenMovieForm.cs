using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.MovieForm;

public static class OpenMovieFormReducers
{
    [ReducerMethod(typeof(StartAddingMovieAction))]
    public static MovieFormState ReduceStartAddingMovieAction(MovieFormState _) =>
        new() { Fetch = ApiCall.Success() };

    [ReducerMethod]
    public static MovieFormState ReduceSelectItemAction(MovieFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Movie
            ? new() { Fetch = ApiCall.InProgress() }
            : state;

    [ReducerMethod(typeof(GoToListItemAction))]
    public static MovieFormState ReduceGoToFranchiseAction(MovieFormState _) =>
        new() { Fetch = ApiCall.Success() };

    [ReducerMethod(typeof(GoToFranchiseComponentAction))]
    public static MovieFormState ReduceGoToFranchiseComponentAction(MovieFormState _) =>
        new() { Fetch = ApiCall.Success() };

}

public sealed class OpenMovieFormEffects
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Movie)
        {
            dispatcher.Dispatch(new FetchMovieAction(action.Item.Id));
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGoToListItemResult(GoToListItemResultAction action, IDispatcher dispatcher)
    {
        action.Handle(
            onSuccess: item =>
            {
                if (item.Type == ListItemType.Movie)
                {
                    dispatcher.Dispatch(new FetchMovieAction(item.Id));
                }
            },
            onFailure: _ => { });

        return Task.CompletedTask;
    }
}
