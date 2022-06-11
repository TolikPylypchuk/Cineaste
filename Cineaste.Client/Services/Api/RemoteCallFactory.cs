namespace Cineaste.Client.Services.Api;

[AutoConstructor]
public sealed partial class RemoteCallFactory : IRemoteCallFactory
{
    private readonly IServiceProvider serviceProvider;

    public RemoteCall<T> Create<TApi, T>(Func<TApi, Task<IApiResponse<T>>> call)
        where TApi : IApi
    {
        var api = this.serviceProvider.GetRequiredService<TApi>();
        return RemoteCall.Create(() => call(api));
    }
}
