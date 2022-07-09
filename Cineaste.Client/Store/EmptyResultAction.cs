namespace Cineaste.Client.Store;

public abstract record EmptyResultAction
{
    public EmptyApiResult Result { get; }

    protected EmptyResultAction(EmptyApiResult result)
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
}
