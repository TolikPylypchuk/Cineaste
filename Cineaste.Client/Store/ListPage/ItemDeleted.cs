namespace Cineaste.Client.Store.ListPage;

using Cineaste.Client.Store.Forms.MovieForm;

public static class ItemDeletedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceDeleteMovieResultAction(ListPageState state, DeleteMovieResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Container = state.Container.RemoveItem(action.Id),
                    SelectedItem = null,
                    SelectionMode = ListPageSelectionMode.None
                },
            onFailure: _ => state);
}
