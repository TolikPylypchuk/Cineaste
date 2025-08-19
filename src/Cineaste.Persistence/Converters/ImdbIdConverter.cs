using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Converters;

internal sealed class ImdbIdConverter : ValueConverter<ImdbId, string>
{
    public ImdbIdConverter()
        : base(id => id.Value, id => new ImdbId(id), null)
    { }
}
