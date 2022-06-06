namespace Cineaste.Client.Services.Api;

[AutoConstructor]
public sealed partial class RemoteCallFactory : IRemoteCallFactory
{
    private readonly IServiceProvider serviceProvider;

    public RemoteCall<T> Create<TApi, T>(Func<TApi, Task<IApiResponse<T>>> call)
        where TApi : IApi
    {
        var api = this.serviceProvider.GetRequiredService<TApi>();

        return RemoteCall.Create(() => this.Execute(api, call));
    }

    private async Task<ApiResult<T>> Execute<TApi, T>(TApi api, Func<TApi, Task<IApiResponse<T>>> call)
    {
        var response = await call(api);

        return response.IsSuccessStatusCode && response.Content is not null
            ? ApiResult.Success(response.Content)
            : response.Error is ValidationApiException ex && ex.Content is not null
                ? ApiResult.Failure<T>(ex.Content)
                : throw new InvalidOperationException("Invalid response from the server", response.Error);
    }
}
