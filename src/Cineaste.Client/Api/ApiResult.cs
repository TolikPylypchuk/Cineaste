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
        if (response.IsSuccessful)
        {
            return response.Content is not null
                ? ApiResult.Success(response.Content)
                : throw new InvalidOperationException("The response content is empty");
        } else if (response.Error is ValidationApiException validationException &&
            validationException.Content is not null)
        {
            return ApiResult.Failure<T>(validationException.Content);
        } else if (response.Error is { } exception &&
            exception.InnerException is TaskCanceledException taskCanceledException)
        {
            throw taskCanceledException;
        } else
        {
            throw new InvalidOperationException("The response error must be of type application/problem+json");
        }
    }

    public static async Task<ApiResult<T>> ToApiResultAsync<T>(this Task<IApiResponse<T>> task) =>
        (await task).ToApiResult();
}
