using System;
using System.Globalization;
using System.Windows.Data;

namespace MovieList.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool boolValue ? !boolValue : throw new NotSupportedException("Can only convert bool values.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => this.Convert(value, targetType, parameter, culture);
    }
}
