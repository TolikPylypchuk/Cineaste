namespace Cineaste.Client.Store.CreateListPage;

public sealed record InitializeCreateListPageAction;

public static class InitializeCreateListPageActionReducers
{
    [ReducerMethod(typeof(InitializeCreateListPageAction))]
    public static CreateListPageState ReduceInitializeCreateListPageAction(CreateListPageState _) =>
        new();
}
