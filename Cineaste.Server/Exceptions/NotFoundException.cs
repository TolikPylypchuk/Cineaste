namespace Cineaste.Server.Exceptions;

public sealed class NotFoundException(
    string resource,
    string? message = null,
    Exception? innerException = null)
    : CineasteException($"NotFound.{resource}", message, innerException)
{
    public string? Resource { get; } = $"Resource.{resource}";
}
