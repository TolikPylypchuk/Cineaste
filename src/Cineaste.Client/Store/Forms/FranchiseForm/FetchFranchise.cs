namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record FetchFranchiseAction(Guid Id);

public sealed record FetchFranchiseResultAction(ApiResult<FranchiseModel> Result)
    : ResultAction<FranchiseModel>(Result);

public static class FetchFranchiseReducers
{
    [ReducerMethod(typeof(FetchFranchiseAction))]
    public static FranchiseFormState ReduceFetchFranchiseAction(FranchiseFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceFetchFranchiseResultAction(
        FranchiseFormState state,
        FetchFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise => state with { Fetch = ApiCall.Success(), Model = franchise },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem) });
}

public sealed class FetchFranchiseEffects(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleFetchFranchise(FetchFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.GetFranchise(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchFranchiseResultAction(result));
    }
}
