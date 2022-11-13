namespace Cineaste.Server.Infrastructure.Problems;

using System.Text.Json.Serialization;

internal sealed class ExceptionDetails
{
    public string Type { get; }
    public string Message { get; }
    public string? Source { get; }
    public IEnumerable<string> StackTrace { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionDetails? InnerException { get; }

    public ExceptionDetails(Exception ex)
    {
        this.Type = ex.GetType().FullName ?? ex.GetType().Name;
        this.Message = ex.Message;
        this.Source = ex.Source;

        this.StackTrace = ex.StackTrace?.Split('\n')
            .Select(stackFrame => stackFrame.Trim())
            .ToList()
            .AsReadOnly()
            ?? Enumerable.Empty<string>();

        this.InnerException = ex.InnerException is not null ? new ExceptionDetails(ex.InnerException) : null;
    }
}
