namespace Cineaste.Client.Api;

public abstract record ApiResult<T>(bool IsSuccessful);

public sealed record ApiSuccess<T>(T Value) : ApiResult<T>(true);

public sealed record ApiFailure<T>(ProblemDetails Problem) : ApiResult<T>(false);

public static class ApiResult
{
    public static ApiSuccess<T> Success<T>(T value) =>
        new(value);

    public static ApiFailure<T> Failure<T>(ProblemDetails problem) =>
        new(problem);
}

public static class ApiResultExtensions
{
    public static ApiResult<T> ToApiResult<T>(this IApiResponse<T> response)
    {
        if (response.IsSuccessStatusCode)
        {
            return response.Content is not null
                ? ApiResult.Success(response.Content)
                : throw new InvalidOperationException("The response's content is empty");
        } else if (response.Error is ValidationApiException exception && exception.Content is not null)
        {
            return ApiResult.Failure<T>(exception.Content);
        } else
        {
            throw new InvalidOperationException("The response's error must be of type application/problem+json");
        }
    }

    public static async Task<ApiResult<T>> ToApiResultAsync<T>(this Task<IApiResponse<T>> task) =>
        (await task).ToApiResult();
}
