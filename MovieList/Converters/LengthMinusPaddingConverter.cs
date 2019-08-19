using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MovieList.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class LengthMinusPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Control control
                ? parameter?.ToString()?.ToLower() == "height"
                    ? control.ActualHeight - control.Padding.Top - control.Padding.Bottom
                    : control.ActualWidth - control.Padding.Left - control.Padding.Right
            : throw new NotSupportedException("Can only convert controls.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Cannot convert back.");
    }
}
