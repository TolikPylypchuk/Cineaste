namespace Cineaste.Client.Api;

public abstract record EmptyApiResult(bool IsSuccessful)
{
    public static EmptyApiSuccess Success { get; } = new();

    public static EmptyApiFailure Failure(ProblemDetails problem) =>
        new(problem);
}

public sealed record EmptyApiSuccess() : EmptyApiResult(true);

public sealed record EmptyApiFailure(ProblemDetails Problem) : EmptyApiResult(false);

public static class EmptyApiResultExtensions
{
    extension(IApiResponse response)
    {
        public EmptyApiResult ToApiResult()
        {
            if (response.IsSuccessStatusCode)
            {
                return EmptyApiResult.Success;
            } else if (response.Error is ValidationApiException exception && exception.Content is not null)
            {
                return EmptyApiResult.Failure(exception.Content);
            } else
            {
                throw new InvalidOperationException("The response's error must be of type application/problem+json");
            }
        }
    }

    extension(Task<IApiResponse> task)
    {
        public async Task<EmptyApiResult> ToApiResultAsync() =>
            (await task).ToApiResult();
    }
}
