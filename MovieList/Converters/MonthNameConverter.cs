using System;
using System.Globalization;
using System.Windows.Data;

using MovieList.Properties;

namespace MovieList.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class MonthNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value switch
            {
                1 => Messages.January,
                2 => Messages.February,
                3 => Messages.March,
                4 => Messages.April,
                5 => Messages.May,
                6 => Messages.June,
                7 => Messages.July,
                8 => Messages.August,
                9 => Messages.September,
                10 => Messages.October,
                11 => Messages.November,
                12 => Messages.December,
                _ => throw new NotSupportedException($"Cannot convert month #{value}.")
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string str
                ? str.ToLower() switch
                {
                    var v when v == Messages.January.ToLower() => 1,
                    var v when v == Messages.February.ToLower() => 2,
                    var v when v == Messages.March.ToLower() => 3,
                    var v when v == Messages.April.ToLower() => 4,
                    var v when v == Messages.May.ToLower() => 5,
                    var v when v == Messages.June.ToLower() => 6,
                    var v when v == Messages.July.ToLower() => 7,
                    var v when v == Messages.August.ToLower() => 8,
                    var v when v == Messages.September.ToLower() => 9,
                    var v when v == Messages.October.ToLower() => 10,
                    var v when v == Messages.November.ToLower() => 11,
                    var v when v == Messages.December.ToLower() => 12,
                    _ => throw new NotSupportedException($"Cannot convert month: {str}.")
                }
                : throw new NotSupportedException($"Cannot convert {value}");
    }
}
