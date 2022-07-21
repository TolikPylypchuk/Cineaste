namespace Cineaste.Server.Exceptions;

public sealed class ConflictException : CineasteException
{
    public string? Resource { get; }

    public ConflictException(
        string resource,
        string? message = null,
        Exception? innerException = null)
        : base($"Conflict.{resource}", message, innerException) =>
        this.Resource = $"Resource.{resource}";
}
