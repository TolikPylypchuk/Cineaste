namespace Cineaste.Client.Services.Api;

public interface IApiExecutor<TApi>
    where TApi : notnull
{
    Task<ApiResult<T>> Fetch<T>(Func<TApi, Task<IApiResponse<T>>> fetch);
}
