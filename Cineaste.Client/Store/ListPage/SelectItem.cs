namespace Cineaste.Client.Store.ListPage;

public sealed record SelectItemAction(ListItemModel Item);

public static class SelectItemReducers
{
    [ReducerMethod]
    public static ListPageState ReduceSelectItemAction(ListPageState state, SelectItemAction action) =>
        state with { SelectedItem = action.Item };
}
