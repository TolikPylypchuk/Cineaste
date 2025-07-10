using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Configuration;

internal sealed class IdConverter<T> : ValueConverter<Id<T>, Guid>
{
    public IdConverter()
        : base(id => id.Value, guid => Id.For<T>(guid), null)
    { }
}
