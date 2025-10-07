using System.ComponentModel;
using System.Globalization;

namespace Cineaste.Core.Converter;

public sealed class IdConverter<T> : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        this.IsTypeSupported(sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
        this.IsTypeSupported(destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
        value switch
        {
            null => null,
            string str => Id.For<T>(Guid.Parse(str)),
            Id<T> id => id.ToString(),
            _ => throw this.GetConvertFromException(value)
        };

    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType) =>
        (value, destinationType) switch
        {
            (null, _) => null,
            (string str, var t) when t == typeof(Id<T>) => Id.For<T>(Guid.Parse(str)),
            (Id<T> id, var t) when t == typeof(string) => id.ToString(),
            _ => throw this.GetConvertToException(value, destinationType)
        };

    private bool IsTypeSupported([NotNullWhen(true)] Type? type) =>
        type is not null && (type == typeof(string) || type == typeof(Id<T>));
}
