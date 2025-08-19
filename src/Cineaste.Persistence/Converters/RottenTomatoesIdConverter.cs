using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Converters;

internal sealed class RottenTomatoesIdConverter : ValueConverter<RottenTomatoesId, string>
{
    public RottenTomatoesIdConverter()
        : base(id => id.Value, id => new RottenTomatoesId(id), null)
    { }
}
