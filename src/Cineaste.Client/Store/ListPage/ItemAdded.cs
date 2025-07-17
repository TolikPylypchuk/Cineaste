using Cineaste.Client.Store.Forms.FranchiseForm;
using Cineaste.Client.Store.Forms.MovieForm;
using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Store.ListPage;

public static class ItemAddedReducers
{
    [ReducerMethod]
    public static ListPageState ReduceAddMovieResultAction(ListPageState state, AddMovieResultAction action) =>
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
    public static ListPageState ReduceAddSeriesResultAction(ListPageState state, AddSeriesResultAction action) =>
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
    public static ListPageState ReduceAddFranchiseResultAction(ListPageState state, AddFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise =>
            {
                var item = franchise.ToListItemModel();
                return state with
                {
                    SelectedItem = item.ShouldBeShown ? item : null,
                    SelectionMode = item.ShouldBeShown ? ListPageSelectionMode.Franchise : ListPageSelectionMode.None
                };
            },
            onFailure: _ => state);
}
