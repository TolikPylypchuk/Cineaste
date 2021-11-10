namespace Cineaste.Converters;

public sealed class UriConverter : IBindingTypeConverter
{
    public int GetAffinityForObjects(Type fromType, Type toType) =>
        fromType == typeof(Uri) || toType == typeof(Uri) ? 10000 : 0;

    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        switch (from)
        {
            case string value:
                result = new Uri(value, UriKind.Absolute);
                return true;
            case Uri value:
                result = value.AbsoluteUri;
                return true;
            case null:
                result = null;
                return true;
            default:
                result = null;
                return false;
        }
    }
}
