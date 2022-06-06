namespace Cineaste.Client.Services.Api;

public interface IRemoteCallFactory
{
    RemoteCall<T> Create<TApi, T>(Func<TApi, Task<IApiResponse<T>>> call) where TApi : IApi;
}