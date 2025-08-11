using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record SetSeriesPosterAction(Guid SeriesId, PosterRequest Request);

public sealed record SetSeriesPosterResultAction(ApiResult<string> Result) : ResultAction<string>(Result);

public sealed record RemoveSeriesPosterAction(Guid SeriesId);

public sealed record RemoveSeriesPosterResultAction(EmptyApiResult Result) : EmptyResultAction(Result);

public static class SetSeriesPosterReducers
{
    [ReducerMethod(typeof(SetSeriesPosterAction))]
    public static SeriesFormState ReduceSetSeriesPosterAction(SeriesFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceSetSeriesPosterResultAction(
        SeriesFormState state,
        SetSeriesPosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = posterUrl } : null,
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveSeriesPosterAction))]
    public static SeriesFormState ReduceRemoveSeriesPosterAction(SeriesFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceRemoveSeriesPosterResultAction(
        SeriesFormState state,
        RemoveSeriesPosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = null } : null,
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });
}

public class SetSeriesPosterEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleSetSeriesPoster(SetSeriesPosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetSeriesPoster(action.SeriesId, file.ToStreamPart()).ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetSeriesPoster(action.SeriesId, urlRequest).ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetSeriesPosterResultAction(result));
    }

    [EffectMethod]
    public async Task HandleRemoveSeriesPoster(RemoveSeriesPosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveSeriesPoster(action.SeriesId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveSeriesPosterResultAction(result));
    }
}
