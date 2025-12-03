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
    extension<T>(IApiResponse<T> response)
    {
        public ApiResult<T> ToApiResult() =>
            response.ToApiResult(r => r.Content is not null
                ? r.Content
                : throw new InvalidOperationException("The response content is empty"));
    }

    extension <R>(R response)
        where R : IApiResponse
    {
        public ApiResult<T> ToApiResult<T>(Func<R, T> onSucces)
        {
            if (response.IsSuccessful)
            {
                return ApiResult.Success(onSucces(response));
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
    }

    extension<T>(Task<IApiResponse<T>> task)
    {
        public async Task<ApiResult<T>> ToApiResultAsync() =>
            (await task).ToApiResult();
    }

    extension<R>(Task<R> task)
        where R : IApiResponse
    {
        public async Task<ApiResult<T>> ToApiResultAsync<T>(Func<R, T> onSuccess) =>
            (await task).ToApiResult(onSuccess);
    }
}
