namespace Cineaste.Server.Exceptions;

using System.Runtime.Serialization;

public sealed class NotFoundException : ApplicationException
{
    public string? Resource { get; }
    public IDictionary<string, object?>? Properties { get; }

    public NotFoundException(
        string? message = null,
        string? resource = null,
        IDictionary<string, object?>? properties = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        this.Resource = resource;
        this.Properties = properties;
    }

    private NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}
