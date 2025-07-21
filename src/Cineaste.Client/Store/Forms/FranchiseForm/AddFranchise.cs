namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record AddFranchiseAction(FranchiseRequest Request);

public sealed record AddFranchiseResultAction(ApiResult<FranchiseModel> Result)
    : ResultAction<FranchiseModel>(Result);

public static class AddFranchiseReducers
{
    [ReducerMethod(typeof(AddFranchiseAction))]
    public static FranchiseFormState ReduceAddFranchiseAction(FranchiseFormState state) =>
        state with { Add = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceAddFranchiseResultAction(
        FranchiseFormState state,
        AddFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise => state with { Add = ApiCall.Success(), Model = franchise },
            onFailure: problem => state with { Add = ApiCall.Failure(problem) });
}

public sealed class AddFranchiseEffects(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleAddFranchise(AddFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.AddFranchise(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new AddFranchiseResultAction(result));
    }
}
