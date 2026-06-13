using Cineaste.Client.Store.Forms.LimitedSeriesForm;

namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record ConvertToLimitedSeriesAction(Guid SeriesId);

public sealed record ConvertToLimitedSeriesResultAction(ApiResult<LimitedSeriesModel> Result)
    : ResultAction<LimitedSeriesModel>(Result);

public static class ConvertToLimitedSeriesReducers
{
    [ReducerMethod(typeof(ConvertToLimitedSeriesAction))]
    public static SeriesFormState ReduceConvertToLimitedSeriesAction(SeriesFormState state) =>
        state with { Convert = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceConvertToLimitedSeriesResultAction(
        SeriesFormState state,
        ConvertToLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Convert = ApiCall.Success() },
            onFailure: problem => state with { Convert = ApiCall.Failure(problem) });

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceConvertToLimitedSeriesResultAction(
        LimitedSeriesFormState state,
        ConvertToLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Model = series },
            onFailure: _ => state);
}

public sealed class ConvertToLimitedSeriesEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleConvertToLimitedSeries(ConvertToLimitedSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.ConvertToLimitedSeries(action.SeriesId).ToApiResultAsync();
        dispatcher.Dispatch(new ConvertToLimitedSeriesResultAction(result));
    }
}
