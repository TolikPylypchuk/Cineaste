using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ReactiveUI;

namespace MovieList.Converters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToBrushConverter : IValueConverter, IBindingTypeConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
            => value is Color color ? new SolidColorBrush(color) : Binding.DoNothing;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
            => value is SolidColorBrush brush ? brush.Color : Binding.DoNothing;

        public int GetAffinityForObjects(Type fromType, Type toType)
            => (fromType, toType) switch
            {
                var (from, to) when from == typeof(Color) && to == typeof(SolidColorBrush) => 10000,
                var (from, to) when from == typeof(SolidColorBrush) && to == typeof(Color) => 10000,
                var (from, to) when from == typeof(Color) && to == typeof(Brush) => 100,
                var (from, to) when from == typeof(Brush) && to == typeof(Color) => 100,
                _ => 0
            };

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            result = toType == typeof(Color)
                ? this.Convert(from, toType, conversionHint, null)
                : this.ConvertBack(from, toType, conversionHint, null);

            return result != Binding.DoNothing;
        }
    }
}
