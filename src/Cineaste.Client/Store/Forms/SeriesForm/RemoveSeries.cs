namespace Cineaste.Client.Store.Forms.SeriesForm;

public record RemoveSeriesAction(Guid Id);

public record RemoveSeriesResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class RemoveSeriesReducers
{
    [ReducerMethod(typeof(RemoveSeriesAction))]
    public static SeriesFormState ReduceRemoveSeriesAction(SeriesFormState state) =>
        state with { Remove = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceRemoveSeriesResultAction(
        SeriesFormState state,
        RemoveSeriesResultAction action) =>
        action.Handle(
            onSuccess: () => new SeriesFormState(),
            onFailure: problem => state with { Remove = ApiCall.Failure(problem) });
}

public sealed class RemoveSeriesEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleRemoveSeriesAction(RemoveSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveSeriesResultAction(action.Id, result));
    }
}
