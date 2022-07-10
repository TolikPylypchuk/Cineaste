namespace Cineaste.Client.Store.ListPage;

using Cineaste.Client.Store.Forms.MovieForm;

public static class ItemUpdatedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceUpdateMovieResultAction(ListPageState state, UpdateMovieResultAction action) =>
        action.Handle(
            onSuccess: movie =>
            {
                var item = movie.ToListItemModel();
                return state with
                {
                    Container = state.Container.UpdateItem(item),
                    SelectedItem = item,
                    SelectionMode = ListPageSelectionMode.Movie
                };
            },
            onFailure: _ => state);
}
