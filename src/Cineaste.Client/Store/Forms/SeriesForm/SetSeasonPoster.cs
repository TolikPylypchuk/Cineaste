using System.Diagnostics.CodeAnalysis;

using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record SetSeasonPosterAction(Guid SeriesId, Guid PeriodId, PosterRequest Request);

public sealed record SetSeasonPosterResultAction(Guid PeriodId, ApiResult<string> Result)
    : ResultAction<string>(Result);

public sealed record RemoveSeasonPosterAction(Guid SeriesId, Guid PeriodId);

public sealed record RemoveSeasonPosterResultAction(Guid PeriodId, EmptyApiResult Result) : EmptyResultAction(Result);

public static class SetSeasonPosterReducers
{
    [ReducerMethod(typeof(SetSeasonPosterAction))]
    public static SeriesFormState ReduceSetSeasonPosterAction(SeriesFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceSetSeasonPosterResultAction(
        SeriesFormState state,
        SetSeasonPosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = UpdatePosterUrlForSeason(state.Model, action.PeriodId, posterUrl),
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveSeasonPosterAction))]
    public static SeriesFormState ReduceRemoveSeasonPosterAction(SeriesFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceRemoveSeasonPosterResultAction(
        SeriesFormState state,
        RemoveSeasonPosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = UpdatePosterUrlForSeason(state.Model, action.PeriodId, null),
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });

    [return: NotNullIfNotNull(nameof(series))]
    private static SeriesModel? UpdatePosterUrlForSeason(
        SeriesModel? series,
        Guid periodId,
        string? posterUrl) =>
        series is not null
            ? series with
            {
                Seasons = [.. series.Seasons.Select(season =>
                    season with
                    {
                        Periods = [.. season.Periods.Select(period =>
                            period.Id == periodId ? period with { PosterUrl = posterUrl } : period)]
                    })]
            }
            : null;
}

public class SetSeasonPosterEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleSetSeasonPoster(SetSeasonPosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetSeasonPoster(action.SeriesId, action.PeriodId, file.ToStreamPart())
                .ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetSeasonPoster(action.SeriesId, action.PeriodId, urlRequest)
                .ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetSeasonPosterResultAction(action.PeriodId, result));
    }

    [EffectMethod]
    public async Task HandleRemoveSeasonPoster(RemoveSeasonPosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveSeasonPoster(action.SeriesId, action.PeriodId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveSeasonPosterResultAction(action.PeriodId, result));
    }
}
