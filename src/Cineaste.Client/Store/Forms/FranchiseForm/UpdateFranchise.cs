namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record UpdateFranchiseAction(Guid Id, FranchiseRequest Request);

public sealed record UpdateFranchiseResultAction(ApiResult<FranchiseModel> Result)
    : ResultAction<FranchiseModel>(Result);

public static class UpdateFranchiseReducers
{
    [ReducerMethod(typeof(UpdateFranchiseAction))]
    public static FranchiseFormState ReduceUpdateFranchiseAction(FranchiseFormState state) =>
        state with { Update = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceUpdateFranchiseResultAction(
        FranchiseFormState state,
        UpdateFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise => state with { Update = ApiCall.Success(), Model = franchise },
            onFailure: problem => state with { Update = ApiCall.Failure(problem) });
}

public sealed class UpdateFranchiseEffects(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleUpdateFranchise(UpdateFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.UpdateFranchise(action.Id, action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new UpdateFranchiseResultAction(result));
    }
}
