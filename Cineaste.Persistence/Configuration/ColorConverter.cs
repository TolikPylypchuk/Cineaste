namespace Cineaste.Persistence.Configuration;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

internal sealed class ColorConverter : ValueConverter<Color, string>
{
    public ColorConverter()
        : base(color => color.HexValue, hex => new Color(hex), null)
    { }
}
