using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cineaste.Persistence.Converters;

internal sealed class MonthConverter : ValueConverter<Month, int>
{
    public MonthConverter()
        : base(month => month.Value, month => new Month(month))
    { }
}
