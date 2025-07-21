namespace Cineaste.Client.Store.ListPage;

public sealed record SelectItemAction(ListItemModel Item);

public sealed record StartAddingMovieAction;

public sealed record StartAddingSeriesAction;

public sealed record StartAddingFranchiseAction;

public sealed record CloseItemAction;

public static class SelectItemReducers
{
    [ReducerMethod]
    public static ListPageState ReduceSelectItemAction(ListPageState state, SelectItemAction action) =>
        state with { SelectedItem = action.Item, SelectionMode = action.Item.Type.ToListPageSelectionMode() };

    [ReducerMethod(typeof(StartAddingMovieAction))]
    public static ListPageState ReduceStartAddingMovieAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.Movie };

    [ReducerMethod(typeof(StartAddingSeriesAction))]
    public static ListPageState ReduceStartAddingSeriesAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.Series };

    [ReducerMethod(typeof(StartAddingFranchiseAction))]
    public static ListPageState ReduceStartAddingFranchiseAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.Franchise };

    [ReducerMethod(typeof(CloseItemAction))]
    public static ListPageState ReduceCloseItemAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.None };
}
