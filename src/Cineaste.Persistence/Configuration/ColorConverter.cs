using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Configuration;

internal sealed class ColorConverter : ValueConverter<Color, string>
{
    public ColorConverter()
        : base(color => color.HexValue, hex => new Color(hex), null)
    { }
}
