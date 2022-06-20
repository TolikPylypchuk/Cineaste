namespace Cineaste.Client.Store.ListPage;

public sealed record SelectItemAction(ListItemModel Item);

public sealed record CloseItemAction;

public static class SelectItemReducers
{
    [ReducerMethod]
    public static ListPageState ReduceSelectItemAction(ListPageState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Movie
            ? state with { SelectedItem = action.Item }
            : state;

    [ReducerMethod(typeof(CloseItemAction))]
    public static ListPageState ReduceCloseItemAction(ListPageState state) =>
        state with { SelectedItem = null };
}
