namespace Cineaste.Client.Api;

[AutoConstructor]
public sealed partial class RemoteCall<T> : ReactiveObject
{
    private readonly Func<Task<IApiResponse<T>>> call;

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

        if (result.IsSuccessStatusCode)
        {
            this.Result = result.Content;
        } else if (result.Error is ValidationApiException exception)
        {
            this.Problem = exception.Content;
        }

        this.IsLoading = false;
    }
}

public static class RemoteCall
{
    public static RemoteCall<T> Create<T>(Func<Task<IApiResponse<T>>> call) =>
        new(call);
}
