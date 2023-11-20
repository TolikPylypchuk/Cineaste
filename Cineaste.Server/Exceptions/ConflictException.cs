namespace Cineaste.Server.Exceptions;

public sealed class ConflictException(
    string resource,
    string? message = null,
    Exception? innerException = null)
    : CineasteException($"Conflict.{resource}", message, innerException)
{
    public string? Resource { get; } = $"Resource.{resource}";
}
