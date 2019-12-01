using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class SeriesStatusToStringConverter : IBindingTypeConverter
    {
        private static readonly Dictionary<SeriesStatus, string> StatusToString = new Dictionary<SeriesStatus, string>
        {
            [SeriesStatus.NotStarted] = Messages.SeriesNotStarted,
            [SeriesStatus.Running] = Messages.SeriesRunning,
            [SeriesStatus.Finished] = Messages.SeriesFinished,
            [SeriesStatus.Cancelled] = Messages.SeriesCancelled
        };

        private static readonly Dictionary<string, SeriesStatus> StringToStatus =
            StatusToString.ToDictionary(e => e.Value, e => e.Key);

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(SeriesStatus) && toType == typeof(string) ||
               fromType == typeof(string) && toType == typeof(SeriesStatus)
                ? 10
                : 0;

        public bool TryConvert(object from, Type toType, object conversionHint, out object? result)
        {
            switch (from)
            {
                case SeriesStatus status:
                    result = StatusToString[status];
                    return true;
                case string str:
                    result = StringToStatus[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
