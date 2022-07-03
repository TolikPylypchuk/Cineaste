namespace Cineaste.Client.Store;

public sealed record ResultAction<T>
{
    public ApiResult<T> Result { get; }

    public ResultAction(ApiResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
    }
}

public static class ResultAction
{
    public static ResultAction<T> Create<T>(ApiResult<T> result) =>
        new(result);
}
