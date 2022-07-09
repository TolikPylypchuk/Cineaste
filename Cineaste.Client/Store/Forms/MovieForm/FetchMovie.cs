namespace Cineaste.Client.Store.Forms.MovieForm;

using Cineaste.Client.Store.ListPage;

public sealed record FetchMovieAction(Guid Id, ImmutableList<ListKindModel> AvailableKinds);

public sealed record FetchMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class FetchMovieReducers
{
    [ReducerMethod]
    public static MovieFormState ReduceFetchMovieAction(MovieFormState _, FetchMovieAction action) =>
        new() { IsLoading = true, AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static MovieFormState ReduceSelectItemAction(MovieFormState state, SelectItemAction action) =>
        action.Item.Id == state.MovieModel?.Id ? state : new() { IsLoading = true };

    [ReducerMethod]
    public static MovieFormState ReduceMovieResultAction(MovieFormState state, FetchMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { IsLoading = false, IsLoaded = true, MovieModel = movie },
            onFailure: problem => state with { IsLoading = false, IsLoaded = true, FetchMovieProblem = problem });
}

[AutoConstructor]
public sealed partial class FetchMovieEffect
{
    private readonly IMovieApi api;

    [EffectMethod]
    public async Task HandleFetchMovie(FetchMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchMovieResultAction(result));
    }
}
