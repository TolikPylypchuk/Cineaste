namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record FetchFranchiseAction(Guid Id, ImmutableList<ListKindModel> AvailableKinds);

public sealed record FetchFranchiseResultAction(ApiResult<FranchiseModel> Result)
    : ResultAction<FranchiseModel>(Result);

public static class FetchFranchiseReducers
{
    [ReducerMethod]
    public static FranchiseFormState ReduceFetchFranchiseAction(FranchiseFormState _, FetchFranchiseAction action) =>
        new() { Fetch = ApiCall.InProgress(), AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static FranchiseFormState ReduceFetchFranchiseResultAction(
        FranchiseFormState state,
        FetchFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise => state with { Fetch = ApiCall.Success(), Model = franchise },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem) });
}

[AutoConstructor]
public sealed partial class FetchFranchiseEffect
{
    private readonly IFranchiseApi api;

    [EffectMethod]
    public async Task HandleFetchFranchise(FetchFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetFranchise(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchFranchiseResultAction(result));
    }
}
