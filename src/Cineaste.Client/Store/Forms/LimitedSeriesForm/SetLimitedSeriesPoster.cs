using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public sealed record SetLimitedSeriesPosterAction(Guid LimitedSeriesId, PosterRequest Request);

public sealed record SetLimitedSeriesPosterResultAction(ApiResult<string> Result) : ResultAction<string>(Result);

public sealed record RemoveLimitedSeriesPosterAction(Guid LimitedSeriesId);

public sealed record RemoveLimitedSeriesPosterResultAction(EmptyApiResult Result) : EmptyResultAction(Result);

public static class SetLimitedSeriesPosterReducers
{
    [ReducerMethod(typeof(SetLimitedSeriesPosterAction))]
    public static LimitedSeriesFormState ReduceSetLimitedSeriesPosterAction(LimitedSeriesFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceSetLimitedSeriesPosterResultAction(
        LimitedSeriesFormState state,
        SetLimitedSeriesPosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = posterUrl } : null,
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveLimitedSeriesPosterAction))]
    public static LimitedSeriesFormState ReduceRemoveLimitedSeriesPosterAction(LimitedSeriesFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceRemoveLimitedSeriesPosterResultAction(
        LimitedSeriesFormState state,
        RemoveLimitedSeriesPosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = null } : null,
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });
}

public class SetLimitedSeriesPosterEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleSetLimitedSeriesPoster(SetLimitedSeriesPosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetLimitedSeriesPoster(action.LimitedSeriesId, file.ToStreamPart())
                .ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetLimitedSeriesPoster(action.LimitedSeriesId, urlRequest)
                .ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetLimitedSeriesPosterResultAction(result));
    }

    [EffectMethod]
    public async Task HandleRemoveLimitedSeriesPoster(RemoveLimitedSeriesPosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveLimitedSeriesPoster(action.LimitedSeriesId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveLimitedSeriesPosterResultAction(result));
    }
}
