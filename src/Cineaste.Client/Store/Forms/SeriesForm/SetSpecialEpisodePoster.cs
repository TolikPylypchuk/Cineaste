using System.Diagnostics.CodeAnalysis;

using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record SetSpecialEpisodePosterAction(Guid SeriesId, Guid EpisodeId, PosterRequest Request);

public sealed record SetSpecialEpisodePosterResultAction(Guid EpisodeId, ApiResult<string> Result)
    : ResultAction<string>(Result);

public sealed record RemoveSpecialEpisodePosterAction(Guid SeriesId, Guid EpisodeId);

public sealed record RemoveSpecialEpisodePosterResultAction(Guid EpisodeId, EmptyApiResult Result)
    : EmptyResultAction(Result);

public static class SetSpecialEpisodePosterReducers
{
    [ReducerMethod(typeof(SetSpecialEpisodePosterAction))]
    public static SeriesFormState ReduceSetSpecialEpisodePosterAction(SeriesFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceSetSpecialEpisodePosterResultAction(
        SeriesFormState state,
        SetSpecialEpisodePosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = UpdatePosterUrlForSpecialEpisode(state.Model, action.EpisodeId, posterUrl),
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveSpecialEpisodePosterAction))]
    public static SeriesFormState ReduceRemoveSpecialEpisodePosterAction(SeriesFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceRemoveSpecialEpisodePosterResultAction(
        SeriesFormState state,
        RemoveSpecialEpisodePosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = UpdatePosterUrlForSpecialEpisode(state.Model, action.EpisodeId, null),
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });

    [return: NotNullIfNotNull(nameof(series))]
    private static SeriesModel? UpdatePosterUrlForSpecialEpisode(
        SeriesModel? series,
        Guid episodeId,
        string? posterUrl) =>
        series is not null
            ? series with
            {
                SpecialEpisodes = [.. series.SpecialEpisodes.Select(episode =>
                    episode.Id == episodeId ? episode with { PosterUrl = posterUrl } : episode)]
            }
            : null;
}

public class SetSpecialEpisodePosterEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleSetSpecialEpisodePoster(SetSpecialEpisodePosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetSpecialEpisodePoster(action.SeriesId, action.EpisodeId, file.ToStreamPart())
                .ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetSpecialEpisodePoster(action.SeriesId, action.EpisodeId, urlRequest)
                .ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetSpecialEpisodePosterResultAction(action.EpisodeId, result));
    }

    [EffectMethod]
    public async Task HandleRemoveSpecialEpisodePoster(RemoveSpecialEpisodePosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveSpecialEpisodePoster(action.SeriesId, action.EpisodeId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveSpecialEpisodePosterResultAction(action.EpisodeId, result));
    }
}
