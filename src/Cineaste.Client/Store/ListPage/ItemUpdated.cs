using Cineaste.Client.Store.Forms.FranchiseForm;
using Cineaste.Client.Store.Forms.MovieForm;
using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Store.ListPage;

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
                    SelectedItem = item,
                    SelectionMode = ListPageSelectionMode.Movie
                };
            },
            onFailure: _ => state);

    [ReducerMethod]
    public static ListPageState ReduceUpdateSeriesResultAction(ListPageState state, UpdateSeriesResultAction action) =>
        action.Handle(
            onSuccess: series =>
            {
                var item = series.ToListItemModel();
                return state with
                {
                    SelectedItem = item,
                    SelectionMode = ListPageSelectionMode.Series
                };
            },
            onFailure: _ => state);

    [ReducerMethod]
    public static ListPageState ReduceUpdateFranchiseResultAction(
        ListPageState state,
        UpdateFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise =>
                state with
                {
                    SelectedItem = franchise.ShowTitles ? franchise.ToListItemModel() : null,
                    SelectionMode = franchise.ShowTitles ? ListPageSelectionMode.Franchise : ListPageSelectionMode.None
                },
            onFailure: _ => state);
}
