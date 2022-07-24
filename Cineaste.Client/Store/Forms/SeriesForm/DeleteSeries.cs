namespace Cineaste.Client.Store.Forms.SeriesForm;

public record DeleteSeriesAction(Guid Id);

public record DeleteSeriesResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class DeleteSeriesReducers
{
    [ReducerMethod(typeof(DeleteSeriesAction))]
    public static SeriesFormState ReduceDeleteSeriesAction(SeriesFormState state) =>
        state with { IsDeletingSeries = true };

    [ReducerMethod]
    public static SeriesFormState ReduceDeleteSeriesResultAction(
        SeriesFormState state,
        DeleteSeriesResultAction action) =>
        action.Handle(
            onSuccess: () => new SeriesFormState(),
            onFailure: problem => state with { IsDeletingSeries = false, DeleteSeriesProblem = problem });
}

[AutoConstructor]
public sealed partial class DeleteSeriesEffect
{
    private readonly ISeriesApi api;

    [EffectMethod]
    public async Task HandleDeleteSeriesAction(DeleteSeriesAction action, IDispatcher dispatcher)
    {
        var result = await this.api.DeleteSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new DeleteSeriesResultAction(action.Id, result));
    }
}
