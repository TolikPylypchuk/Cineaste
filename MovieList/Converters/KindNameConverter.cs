using System;
using System.Globalization;
using System.Windows.Data;

namespace MovieList.Converters
{
    public class KindNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string name && parameter is string format
                ? String.Format(format, name)
                : throw new NotSupportedException("Value and paramter for converting kind names must both be strings.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Converting kind names back is not supported.");
    }
}
