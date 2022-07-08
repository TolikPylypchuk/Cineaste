namespace Cineaste.Client.Store.CreateListPage;

public sealed record FetchCulturesAction;

public sealed record FetchCulturesResultAction(ApiResult<List<SimpleCultureModel>> Result)
    : ResultAction<List<SimpleCultureModel>>(Result);

public static class FetchCulturesReducers
{
    [ReducerMethod(typeof(FetchCulturesAction))]
    public static CreateListPageState ReduceFetchCulturesAction(CreateListPageState state) =>
        state;

    [ReducerMethod]
    public static CreateListPageState ReduceFetchCulturesResultAction(
        CreateListPageState state,
        FetchCulturesResultAction action) =>
        action.Handle(
            onSuccess: cultures => state with { AllCultures = cultures.ToImmutableList() },
            onFailure: problem => state with { CulturesProblem = problem });
}

[AutoConstructor]
public sealed partial class FetchListsEffect
{
    private readonly ICultureApi api;

    [EffectMethod(typeof(FetchCulturesAction))]
    public async Task HandleFetchListsAction(IDispatcher dispatcher)
    {
        var result = await this.api.GetAllCultures().ToApiResultAsync();
        dispatcher.Dispatch(new FetchCulturesResultAction(result));
    }
}
