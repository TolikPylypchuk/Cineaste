namespace Cineaste.Server.Exceptions;

public sealed class NotFoundException : CineasteException
{
    public string? Resource { get; }

    public NotFoundException(
        string resource,
        string? message = null,
        Exception? innerException = null)
        : base($"NotFound.{resource}", message, innerException) =>
        this.Resource = $"Resource.{resource}";
}
