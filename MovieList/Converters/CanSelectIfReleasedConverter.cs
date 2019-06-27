using System;
using System.Globalization;
using System.Windows.Data;

using MovieList.ViewModels.FormItems;

namespace MovieList.Converters
{
    [ValueConversion(typeof(MovieFormItem), typeof(bool))]
    [ValueConversion(typeof(SeasonFormItem), typeof(bool))]
    [ValueConversion(typeof(SpecialEpisodeFormItem), typeof(bool))]
    public class CanSelectIfReleasedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value switch
            {
                MovieFormItem movie when Int32.TryParse(movie.Year, out int year) => year == DateTime.Now.Year,
                MovieFormItem movie when String.IsNullOrEmpty(movie.Year) => true,

                SeasonFormItem season when Int32.TryParse(season.Periods[0].StartYear, out int year) =>
                    year >= DateTime.Now.Year ||
                    (year == DateTime.Now.Year && season.Periods[0].StartMonth >= DateTime.Now.Month),
                SeasonFormItem season when String.IsNullOrEmpty(season.Periods[0].StartYear) => true,

                SpecialEpisodeFormItem episode when Int32.TryParse(episode.Year, out int year) =>
                    year >= DateTime.Now.Year ||
                    (year == DateTime.Now.Year && episode.Month >= DateTime.Now.Month),
                SpecialEpisodeFormItem episode when String.IsNullOrEmpty(episode.Year) => true,

                null => false,

                _ => throw new NotSupportedException($"Cannot convert {value}")
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("Cannot convert back.");
    }
}
