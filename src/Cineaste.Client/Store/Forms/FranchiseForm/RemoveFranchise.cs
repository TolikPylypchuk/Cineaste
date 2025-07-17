namespace Cineaste.Client.Store.Forms.FranchiseForm;

public record RemoveFranchiseAction(Guid Id);

public record RemoveFranchiseResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class RemoveFranchiseReducers
{
    [ReducerMethod(typeof(RemoveFranchiseAction))]
    public static FranchiseFormState ReduceRemoveFranchiseAction(FranchiseFormState state) =>
        state with { Remove = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceRemoveFranchiseResultAction(
        FranchiseFormState state,
        RemoveFranchiseResultAction action) =>
        action.Handle(
            onSuccess: () => new FranchiseFormState(),
            onFailure: problem => state with { Remove = ApiCall.Failure(problem) });
}

public sealed class RemoveFranchiseEffect(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleRemoveFranchiseAction(RemoveFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveFranchise(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveFranchiseResultAction(action.Id, result));
    }
}
