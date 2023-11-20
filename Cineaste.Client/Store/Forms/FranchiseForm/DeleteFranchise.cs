namespace Cineaste.Client.Store.Forms.FranchiseForm;

public record DeleteFranchiseAction(Guid Id);

public record DeleteFranchiseResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class DeleteFranchiseReducers
{
    [ReducerMethod(typeof(DeleteFranchiseAction))]
    public static FranchiseFormState ReduceDeleteFranchiseAction(FranchiseFormState state) =>
        state with { Delete = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceDeleteFranchiseResultAction(
        FranchiseFormState state,
        DeleteFranchiseResultAction action) =>
        action.Handle(
            onSuccess: () => new FranchiseFormState(),
            onFailure: problem => state with { Delete = ApiCall.Failure(problem) });
}

public sealed class DeleteFranchiseEffect(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleDeleteFranchiseAction(DeleteFranchiseAction action, IDispatcher dispatcher)
    {
        var result = await api.DeleteFranchise(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new DeleteFranchiseResultAction(action.Id, result));
    }
}
