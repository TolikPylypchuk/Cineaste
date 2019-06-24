using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieList.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool visible
                ? (visible ? Visibility.Visible : Visibility.Hidden)
                : throw new NotSupportedException($"Cannot convert {value}.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Cannot convert back.");
    }
}
