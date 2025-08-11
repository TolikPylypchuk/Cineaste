using static Cineaste.Client.Store.Forms.PosterUtils;

namespace Cineaste.Client.Store.Forms.FranchiseForm;

public sealed record SetFranchisePosterAction(Guid FranchiseId, PosterRequest Request);

public sealed record SetFranchisePosterResultAction(ApiResult<string> Result) : ResultAction<string>(Result);

public sealed record RemoveFranchisePosterAction(Guid FranchiseId);

public sealed record RemoveFranchisePosterResultAction(EmptyApiResult Result) : EmptyResultAction(Result);

public static class SetFranchisePosterReducers
{
    [ReducerMethod(typeof(SetFranchisePosterAction))]
    public static FranchiseFormState ReduceSetFranchisePosterAction(FranchiseFormState state) =>
        state with { SetPoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceSetFranchisePosterResultAction(
        FranchiseFormState state,
        SetFranchisePosterResultAction action) =>
        action.Handle(
            onSuccess: posterUrl =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = posterUrl } : null,
                    SetPoster = ApiCall.Success()
                },
            onFailure: problem => state with { SetPoster = ApiCall.Failure(problem) });

    [ReducerMethod(typeof(RemoveFranchisePosterAction))]
    public static FranchiseFormState ReduceRemoveFranchisePosterAction(FranchiseFormState state) =>
        state with { RemovePoster = ApiCall.InProgress() };

    [ReducerMethod]
    public static FranchiseFormState ReduceRemoveFranchisePosterResultAction(
        FranchiseFormState state,
        RemoveFranchisePosterResultAction action) =>
        action.Handle(
            onSuccess: () =>
                state with
                {
                    Model = state.Model is not null ? state.Model with { PosterUrl = null } : null,
                    RemovePoster = ApiCall.Success()
                },
            onFailure: problem => state with { RemovePoster = ApiCall.Failure(problem) });
}

public class SetFranchisePosterEffects(IFranchiseApi api)
{
    [EffectMethod]
    public async Task HandleSetFranchisePoster(SetFranchisePosterAction action, IDispatcher dispatcher)
    {
        var result = await action.Request.Select(
            file => api.SetFranchisePoster(action.FranchiseId, file.ToStreamPart()).ToApiResultAsync(GetPosterLocation),
            urlRequest => api.SetFranchisePoster(action.FranchiseId, urlRequest).ToApiResultAsync(GetPosterLocation));

        dispatcher.Dispatch(new SetFranchisePosterResultAction(result));
    }

    [EffectMethod]
    public async Task HandleRemoveFranchisePoster(RemoveFranchisePosterAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveFranchisePoster(action.FranchiseId).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveFranchisePosterResultAction(result));
    }
}
