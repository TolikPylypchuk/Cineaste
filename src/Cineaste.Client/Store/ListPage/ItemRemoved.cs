using Cineaste.Client.Store.Forms.MovieForm;
using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Store.ListPage;

public static class ItemRemovedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceRemoveMovieResultAction(ListPageState state, RemoveMovieResultAction action) =>
        ReduceRemoveResultAction(state, action);

    [ReducerMethod]
    public static ListPageState ReduceRemoveSeriesResultAction(ListPageState state, RemoveSeriesResultAction action) =>
        ReduceRemoveResultAction(state, action);

    private static ListPageState ReduceRemoveResultAction(ListPageState state, EmptyResultAction action) =>
        action.Handle(
            onSuccess: () => state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.None },
            onFailure: _ => state);
}
