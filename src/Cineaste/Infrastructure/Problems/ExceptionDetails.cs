namespace Cineaste.Infrastructure.Problems;

using System.Text.Json.Serialization;

internal sealed class ExceptionDetails(Exception ex)
{
    public string Type { get; } = ex.GetType().FullName ?? ex.GetType().Name;

    public string Message { get; } = ex.Message;

    public string? Source { get; } = ex.Source;

    public IEnumerable<string> StackTrace { get; } =
        ex.StackTrace?.Split('\n')
            .Select(stackFrame => stackFrame.Trim())
            .ToList()
            .AsReadOnly()
            ?? Enumerable.Empty<string>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionDetails? InnerException { get; } =
        ex.InnerException is not null ? new ExceptionDetails(ex.InnerException) : null;
}
