namespace Cineaste.Client.Store.Forms.SeriesForm;

public record DeleteSeriesAction(Guid Id);

public record DeleteSeriesResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class DeleteSeriesReducers
{
    [ReducerMethod(typeof(DeleteSeriesAction))]
    public static SeriesFormState ReduceDeleteSeriesAction(SeriesFormState state) =>
        state with { Delete = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceDeleteSeriesResultAction(
        SeriesFormState state,
        DeleteSeriesResultAction action) =>
        action.Handle(
            onSuccess: () => new SeriesFormState(),
            onFailure: problem => state with { Delete = ApiCall.Failure(problem) });
}

public sealed class DeleteSeriesEffect(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleDeleteSeriesAction(DeleteSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.DeleteSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new DeleteSeriesResultAction(action.Id, result));
    }
}
