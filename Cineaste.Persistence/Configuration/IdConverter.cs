namespace Cineaste.Persistence.Configuration;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

internal sealed class IdConverter<T> : ValueConverter<Id<T>, Guid>
{
    public IdConverter()
        : base(id => id.Value, guid => Id.For<T>(guid), null)
    { }
}
