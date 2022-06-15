namespace Cineaste.Client.Store.CreateListPage;

public sealed record FetchCulturesAction;

public sealed record FetchCulturesResultAction(ApiResult<List<SimpleCultureModel>> Result);

public static class FetchCulturesReducers
{
    [ReducerMethod(typeof(FetchCulturesAction))]
    public static CreateListPageState ReduceFetchCulturesAction(CreateListPageState state) =>
        state;

    [ReducerMethod]
    public static CreateListPageState ReduceFetchCulturesResultAction(
        CreateListPageState state,
        FetchCulturesResultAction action) =>
        action.Result switch
        {
            ApiSuccess<List<SimpleCultureModel>> success =>
                state with { AllCultures = success.Value.ToImmutableList() },
            ApiFailure<List<SimpleCultureModel>> failure =>
                state with { CulturesProblem = failure.Problem },
            _ => state
        };
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
