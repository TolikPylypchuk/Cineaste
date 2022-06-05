namespace Cineaste.Client.Services.Api;

[AutoConstructor]
public sealed partial class ApiExecutor<TApi> : IApiExecutor<TApi>
    where TApi : notnull
{
    private readonly TApi api;

    public async Task<ApiResult<T>> Fetch<T>(Func<TApi, Task<IApiResponse<T>>> fetch)
    {
        var response = await fetch(this.api);

        return response.IsSuccessStatusCode && response.Content is not null
            ? ApiResult.Success(response.Content)
            : response.Error is ValidationApiException ex && ex.Content is not null
                ? ApiResult.Failure<T>(ex.Content)
                : throw new InvalidOperationException("Invalid response from the server");
    }
}
