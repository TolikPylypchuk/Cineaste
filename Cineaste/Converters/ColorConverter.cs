using System;

using Avalonia.Media;

using FluentAvalonia.UI.Media;

using ReactiveUI;

namespace Cineaste.Converters
{
    public sealed class ColorConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType) =>
            fromType == typeof(Color) || toType == typeof(Color) ||
                fromType == typeof(Color2) || toType == typeof(Color2)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case string value when toType == typeof(Color) && Color.TryParse(value, out var color):
                    result = color;
                    return true;
                case string value when toType == typeof(Color2) && Color2.TryParse(value, out var color):
                    result = color;
                    return true;
                case Color value:
                    result = $"#{value.ToUint32():X8}";
                    return true;
                case Color2 value:
                    result = value.ToHexString();
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
}
