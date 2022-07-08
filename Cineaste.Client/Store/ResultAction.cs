namespace Cineaste.Client.Store;

public abstract record ResultAction<T>
{
    public ApiResult<T> Result { get; }

    protected ResultAction(ApiResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
    }

    public TResult Handle<TResult>(
        Func<T, TResult> onSuccess,
        Func<ProblemDetails, TResult> onFailure) =>
        this.Result switch
        {
            ApiSuccess<T> success => onSuccess(success.Value),
            ApiFailure<MovieModel> failure => onFailure(failure.Problem),
            _ => Match.ImpossibleType<TResult>(this.Result)
        };
}
