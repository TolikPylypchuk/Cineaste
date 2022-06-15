namespace Cineaste.Client.Store.ListPage;

public sealed record ResetListPageAction;

public static class ResetListPageReducers
{
    [ReducerMethod(typeof(ResetListPageAction))]
    public static ListPageState ReduceResetListPageAction(ListPageState _) =>
        new();
}
