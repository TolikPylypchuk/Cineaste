namespace Cineaste.Client.Store.ListPage;

public sealed record SelectItemAction(
    ListItemModel Item,
    ImmutableList<ListKindModel> AvailableKinds,
    ListConfigurationModel ListConfiguration);

public sealed record StartCreatingMovieAction(ImmutableList<ListKindModel> AvailableKinds);

public sealed record StartCreatingSeriesAction(
    ImmutableList<ListKindModel> AvailableKinds,
    ListConfigurationModel ListConfiguration);

public sealed record StartCreatingFranchiseAction(ImmutableList<ListKindModel> AvailableKinds);

public sealed record CloseItemAction;

public static class SelectItemReducers
{
    [ReducerMethod]
    public static ListPageState ReduceSelectItemAction(ListPageState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Movie || action.Item.Type == ListItemType.Series
            ? state with { SelectedItem = action.Item, SelectionMode = GetSelectionMode(action) }
            : state with { SelectedItem = null, SelectionMode = GetSelectionMode(action) };

    [ReducerMethod(typeof(StartCreatingMovieAction))]
    public static ListPageState ReduceStartCreatingMovieAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.Movie };

    [ReducerMethod(typeof(StartCreatingSeriesAction))]
    public static ListPageState ReduceStartCreatingSeriesAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.Series };

    [ReducerMethod(typeof(CloseItemAction))]
    public static ListPageState ReduceCloseItemAction(ListPageState state) =>
        state with { SelectedItem = null, SelectionMode = ListPageSelectionMode.None };

    private static ListPageSelectionMode GetSelectionMode(SelectItemAction action) =>
        action.Item?.Type switch
        {
            ListItemType.Movie => ListPageSelectionMode.Movie,
            ListItemType.Series => ListPageSelectionMode.Series,
            ListItemType.Franchise => ListPageSelectionMode.Franchise,
            _ => ListPageSelectionMode.None
        };
}
