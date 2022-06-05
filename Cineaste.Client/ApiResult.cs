namespace Cineaste.Client;

public abstract class ApiResult<T>
{ }

public static class ApiResult
{
    public static ApiSuccess<T> Success<T>(T content) =>
        new(content);

    public static ApiFailure<T> Failure<T>(ProblemDetails problem) =>
        new(problem);
}

public sealed class ApiSuccess<T> : ApiResult<T>
{
    public T Content { get; }

    public ApiSuccess(T content) =>
        this.Content = content;
}

public sealed class ApiFailure<T> : ApiResult<T>
{
    public ProblemDetails Problem { get; }

    public ApiFailure(ProblemDetails problem) =>
        this.Problem = problem;
}
