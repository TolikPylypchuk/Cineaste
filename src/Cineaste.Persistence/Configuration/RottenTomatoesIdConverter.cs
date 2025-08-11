using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Configuration;

internal sealed class RottenTomatoesIdConverter : ValueConverter<RottenTomatoesId, string>
{
    public RottenTomatoesIdConverter()
        : base(id => id.Value, id => new RottenTomatoesId(id), null)
    { }
}
