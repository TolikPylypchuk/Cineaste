namespace Cineaste.Client.Api;

[AutoConstructor]
public sealed partial class RemoteCall<T> : ReactiveObject
{
    private readonly Func<Task<ApiResult<T>>> call;

    [Reactive]
    public bool IsLoading { get; private set; }

    public T? Result { get; private set; }

    [Reactive]
    public ProblemDetails? Problem { get; private set; }

    public async Task Execute()
    {
        this.IsLoading = true;
        this.Result = default;
        this.Problem = default;

        var result = await this.call();

        if (result is ApiSuccess<T> successfulResult)
        {
            this.Result = successfulResult.Content;
        } else if (result is ApiFailure<T> failedResult)
        {
            this.Problem = failedResult.Problem;
        }

        this.IsLoading = false;
    }
}

public static class RemoteCall
{
    public static RemoteCall<T> Create<T>(Func<Task<ApiResult<T>>> call) =>
        new(call);
}
