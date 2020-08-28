using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ReactiveUI;

namespace MovieList.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(string))]
    public class BrushToHexConverter : IValueConverter, IBindingTypeConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
            => value is SolidColorBrush brush
                ? $"#{LowerHexString(brush.Color.R) + LowerHexString(brush.Color.G) + LowerHexString(brush.Color.B)}"
                : null;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
            => value is string color
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(color))
                : null;

        public int GetAffinityForObjects(Type fromType, Type toType)
            => (fromType, toType) switch
            {
                var (from, to) when from == typeof(string) && to == typeof(SolidColorBrush) => 10000,
                var (from, to) when from == typeof(SolidColorBrush) && to == typeof(string) => 10000,
                var (from, to) when from == typeof(string) && to == typeof(Brush) => 100,
                var (from, to) when from == typeof(Brush) && to == typeof(string) => 100,
                _ => 0
            };

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            result = toType == typeof(string)
                ? this.Convert(from, toType, conversionHint, null)
                : this.ConvertBack(from, toType, conversionHint, null);

            return result != null;
        }

        private static string LowerHexString(int num)
            => num.ToString("X2").ToLower();
    }
}
