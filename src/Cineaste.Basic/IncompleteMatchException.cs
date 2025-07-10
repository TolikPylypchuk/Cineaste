using System.Diagnostics.CodeAnalysis;

namespace Cineaste.Basic;

[ExcludeFromCodeCoverage]
public sealed class IncompleteMatchException : Exception
{
    public IncompleteMatchException()
    { }

    public IncompleteMatchException(string? message)
        : base(message)
    { }

    public IncompleteMatchException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
