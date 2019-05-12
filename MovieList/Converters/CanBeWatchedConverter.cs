using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using MovieList.Data.Models;

namespace MovieList.Converters
{
    [ValueConversion(typeof(Movie), typeof(bool))]
    [ValueConversion(typeof(Season), typeof(bool))]
    public class CanBeWatchedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value switch
            {
                Movie movie => movie.Year <= DateTime.Now.Year,
                Season season =>
                    season.GetOrderedPeriods().First().StartYear <= DateTime.Now.Year &&
                    season.GetOrderedPeriods().First().StartMonth <= DateTime.Now.Month,
                _ => throw new NotSupportedException($"Cannot convert {value}")
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Cannot convert back.");
    }
}
