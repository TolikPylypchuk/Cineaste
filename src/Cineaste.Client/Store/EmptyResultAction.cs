namespace Cineaste.Client.Store;

public abstract record EmptyResultAction : ResultAction
{
    public EmptyApiResult Result { get; }

    protected EmptyResultAction(EmptyApiResult result)
        : base(result.IsSuccessful)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
    }

    public TResult Handle<TResult>(
        Func<TResult> onSuccess,
        Func<ProblemDetails, TResult> onFailure) =>
        this.Result switch
        {
            EmptyApiSuccess => onSuccess(),
            EmptyApiFailure failure => onFailure(failure.Problem),
            _ => Match.ImpossibleType<TResult>(this.Result)
        };

    public void Handle(
        Action onSuccess,
        Action<ProblemDetails> onFailure)
    {
        switch (this.Result)
        {
            case EmptyApiSuccess:
                onSuccess();
                break;
            case EmptyApiFailure failure:
                onFailure(failure.Problem);
                break;
        }
    }

    public void OnSuccess(Action onSuccess)
    {
        if (this.IsSuccessful)
        {
            onSuccess();
        }
    }
}
