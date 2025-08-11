using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record SetMoviePosterAction(Guid MovieId, PosterRequest Request);

public sealed record SetMoviePosterResultAction(ApiResult<string> Result) : ResultAction<string>(Result);

public sealed record RemoveMoviePosterAction(Guid MovieId);

public sealed record RemoveMoviePosterResultAction(EmptyApiResult Result) : EmptyResultAction(Result);

public static class SetMoviePosterReducers
{
    [ReducerMethod(typeof(SetMoviePosterAction))]
    public static MovieFormState ReduceSetMoviePosterAction(MovieFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceSetMoviePosterResultAction(
        MovieFormState state,
        SetMoviePosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = posterUrl } : null,
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveMoviePosterAction))]
    public static MovieFormState ReduceRemoveMoviePosterAction(MovieFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceRemoveMoviePosterResultAction(
        MovieFormState state,
        RemoveMoviePosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = null } : null,
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });
}

public class SetMoviePosterEffects(IMovieApi api)
{
    [EffectMethod]
    public async Task HandleSetMoviePoster(SetMoviePosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetMoviePoster(action.MovieId, file.ToStreamPart()).ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetMoviePoster(action.MovieId, urlRequest).ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetMoviePosterResultAction(result));
    }

    [EffectMethod]
    public async Task HandleRemoveMoviePoster(RemoveMoviePosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveMoviePoster(action.MovieId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveMoviePosterResultAction(result));
    }
}
