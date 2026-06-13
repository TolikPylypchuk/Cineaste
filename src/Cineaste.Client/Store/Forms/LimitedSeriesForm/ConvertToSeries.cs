using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public sealed record ConvertToSeriesAction(Guid LimitedSeriesId);

public sealed record ConvertToSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class ConvertToSeriesReducers
{
    [ReducerMethod(typeof(ConvertToSeriesAction))]
    public static LimitedSeriesFormState ReduceConvertToSeriesAction(LimitedSeriesFormState state) =>
        state with { Convert = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceConvertToSeriesResultAction(
        LimitedSeriesFormState state,
        ConvertToSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Convert = ApiCall.Success() },
            onFailure: problem => state with { Convert = ApiCall.Failure(problem) });

    [ReducerMethod]
    public static SeriesFormState ReduceConvertToSeriesResultAction(
        SeriesFormState state,
        ConvertToSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Model = series },
            onFailure: _ => state);
}

public sealed class ConvertToSeriesEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleConvertToSeries(ConvertToSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.ConvertToSeries(action.LimitedSeriesId).ToApiResultAsync();
        dispatcher.Dispatch(new ConvertToSeriesResultAction(result));
    }
}
