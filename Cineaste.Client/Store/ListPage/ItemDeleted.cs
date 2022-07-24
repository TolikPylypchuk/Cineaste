namespace Cineaste.Client.Store.ListPage;

using Cineaste.Client.Store.Forms.MovieForm;
using Cineaste.Client.Store.Forms.SeriesForm;

public static class ItemDeletedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceDeleteMovieResultAction(ListPageState state, DeleteMovieResultAction action) =>
        ReduceDeleteResultAction(state, action, action.Id);

    [ReducerMethod]
    public static ListPageState ReduceDeleteSeriesResultAction(ListPageState state, DeleteSeriesResultAction action) =>
        ReduceDeleteResultAction(state, action, action.Id);

    public static ListPageState ReduceDeleteResultAction(ListPageState state, EmptyResultAction action, Guid id) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Container = state.Container.RemoveItem(id),
                    SelectedItem = null,
                    SelectionMode = ListPageSelectionMode.None
                },
            onFailure: _ => state);
}
