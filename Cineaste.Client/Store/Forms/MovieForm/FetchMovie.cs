namespace Cineaste.Client.Store.Forms.MovieForm;

using Cineaste.Client.Store.ListPage;

public sealed record FetchMovieAction(Guid Id);

public sealed record FetchMovieResultAction(ApiResult<MovieModel> Result);

public static class FetchMovieReducers
{
    [ReducerMethod(typeof(FetchMovieAction))]
    public static MovieFormState ReduceFetchMovieAction(MovieFormState _) =>
        new() { IsLoading = true };

    [ReducerMethod]
    public static MovieFormState ReduceSelectItemAction(MovieFormState state, SelectItemAction action)
    {
        return action.Item.Id == state.MovieModel?.Id
            ? state
            : new() { IsLoading = true };
    }

    [ReducerMethod]
    public static MovieFormState ReduceFetchMovieResultAction(MovieFormState state, FetchMovieResultAction action) =>
        action.Result switch
        {
            ApiSuccess<MovieModel> success =>
                new() { IsLoading = false, IsLoaded = true, MovieModel = success.Value },
            ApiFailure<MovieModel> failure =>
                new() { IsLoading = false, IsLoaded = true, Problem = failure.Problem },
            _ => state
        };
}

[AutoConstructor]
public sealed partial class FetchMovieEffect
{
    private readonly IState<MovieFormState> state;
    private readonly IMovieApi api;

    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (state.Value.MovieModel?.Id != action.Item.Id && action.Item.Type == ListItemType.Movie)
        {
            dispatcher.Dispatch(new SetAvailableMovieKindsAction(action.AvailableKinds));
            dispatcher.Dispatch(new FetchMovieAction(action.Item.Id));
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleFetchMovie(FetchMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchMovieResultAction(result));
    }
}
