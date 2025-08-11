namespace Cineaste.Client.Store;

public abstract record ResultAction(bool IsSuccessful);

public abstract record ResultAction<T> : ResultAction
{
    public ApiResult<T> Result { get; }

    protected ResultAction(ApiResult<T> result)
        : base(result?.IsSuccessful ?? false)
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
            ApiFailure<T> failure => onFailure(failure.Problem),
            _ => Match.ImpossibleType<TResult>(this.Result)
        };

    public void Handle(
        Action<T> onSuccess,
        Action<ProblemDetails> onFailure)
    {
        switch (this.Result)
        {
            case ApiSuccess<T> success:
                onSuccess(success.Value);
                break;
            case ApiFailure<T> failure:
                onFailure(failure.Problem);
                break;
        }
    }

    public void OnSuccess(Action<T> onSuccess)
    {
        if (this.Result is ApiSuccess<T> succes)
        {
            onSuccess(succes.Value);
        }
    }
}
