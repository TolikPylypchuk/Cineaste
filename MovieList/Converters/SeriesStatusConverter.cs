using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.Converters
{
    [ValueConversion(typeof(SeriesStatus), typeof(string))]
    public class SeriesStatusConverter : IValueConverter
    {
        private readonly Dictionary<SeriesStatus, string> statusToString;
        private readonly Dictionary<string, SeriesStatus> stringToStatus;

        public SeriesStatusConverter()
        {
            this.statusToString = new Dictionary<SeriesStatus, string>
            {
                [SeriesStatus.NotStarted] = Messages.SeriesNotStarted,
                [SeriesStatus.Running] = Messages.SeriesRunning,
                [SeriesStatus.Finished] = Messages.SeriesFinished,
                [SeriesStatus.Cancelled] = Messages.SeriesCancelled
            };

            this.stringToStatus = this.statusToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is SeriesStatus status
                ? this.statusToString[status]
                : throw new NotSupportedException($"Cannot convert {value}.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string str
                ? this.stringToStatus[str]
                : throw new NotSupportedException($"Cannot convert {value}.");
    }
}
