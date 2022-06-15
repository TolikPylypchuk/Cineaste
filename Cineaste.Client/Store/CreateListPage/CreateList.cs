namespace Cineaste.Client.Store.CreateListPage;

public sealed record CreateListAction(CreateListRequest Request);

public sealed record CreateListFailureAction(ProblemDetails Problem);

public static class CreateListReducers
{
    [ReducerMethod(typeof(CreateListAction))]
    public static CreateListPageState ReduceCreateListAction(CreateListPageState state) =>
        state with { IsCreatingList = true };

    [ReducerMethod]
    public static CreateListPageState ReduceCreateListFailureAction(
        CreateListPageState state,
        CreateListFailureAction action) =>
        state with { IsCreatingList = false, CreateListProblem = action.Problem };
}

[AutoConstructor]
public sealed partial class CreateListEffect
{
    private readonly IListApi api;
    private readonly IPageNavigator pageNavigator;

    [EffectMethod]
    public async Task HandleCreateListAction(CreateListAction action, IDispatcher dispatcher)
    {
        var result = await this.api.CreateList(action.Request).ToApiResultAsync();

        if (result is ApiSuccess<SimpleListModel> success)
        {
            this.pageNavigator.GoToListPage(success.Value.Handle);
        } else if (result is ApiFailure<SimpleListModel> failure)
        {
            dispatcher.Dispatch(new CreateListFailureAction(failure.Problem));
        }
    }
}
