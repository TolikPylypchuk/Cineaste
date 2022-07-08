namespace Cineaste.Client.Store.ListPage;

using Cineaste.Client.Store.Forms.MovieForm;

public static class ItemCreatedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceCreateMovieResultAction(ListPageState state, CreateMovieResultAction action) =>
        action.Handle(
            onSuccess: movie =>
            {
                var item = movie.ToListItemModel();
                return state with
                {
                    Container = state.Container.AddItem(item),
                    SelectedItem = item,
                    SelectionMode = ListPageSelectionMode.Movie
                };
            },
            onFailure: _ => state);
}
