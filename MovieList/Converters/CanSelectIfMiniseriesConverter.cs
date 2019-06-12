using System;
using System.Globalization;
using System.Windows.Data;

using MovieList.ViewModels.FormItems;

namespace MovieList.Converters
{
    [ValueConversion(typeof(SeriesFormItem), typeof(bool))]
    public class CanSelectIfMiniseriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is SeriesFormItem series
                ? series.Series.IsMiniseries || series.Components.Count == 0
                : throw new NotSupportedException($"Cannot convert {value}.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Cannot convert back.");
    }
}
