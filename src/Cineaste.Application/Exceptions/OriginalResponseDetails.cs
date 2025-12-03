namespace Cineaste.Application.Exceptions;

public sealed record OriginalResponseDetails(
    int StatusCode,
    Dictionary<string, object> Headers,
    string Body);
