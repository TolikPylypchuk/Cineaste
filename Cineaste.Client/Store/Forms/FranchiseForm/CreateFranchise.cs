namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record CreateFranchiseAction(FranchiseRequest Request);

public sealed record CreateFranchiseResultAction(ApiResult<FranchiseModel> Result)
    : ResultAction<FranchiseModel>(Result);

public static class CreateFranchiseReducers
{
    [ReducerMethod(typeof(CreateFranchiseAction))]
    public static FranchiseFormState ReduceCreateFranchiseAction(FranchiseFormState state) =>
        state with { Create = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceCreateFranchiseResultAction(
        FranchiseFormState state,
        CreateFranchiseResultAction action) =>
        action.Handle(
            onSuccess: franchise => state with { Create = ApiCall.Success(), Model = franchise },
            onFailure: problem => state with { Create = ApiCall.Failure(problem) });
}

public sealed class CreateFranchiseEffect(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleCreateFranchise(CreateFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.CreateFranchise(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new CreateFranchiseResultAction(result));
    }
}
