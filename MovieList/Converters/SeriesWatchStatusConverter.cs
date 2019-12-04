using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class SeriesWatchStatusConverter : IBindingTypeConverter
    {
        private readonly Dictionary<SeriesWatchStatus, string> statusToString;
        private readonly Dictionary<string, SeriesWatchStatus> stringToStatus;

        public SeriesWatchStatusConverter()
        {
            this.statusToString = new Dictionary<SeriesWatchStatus, string>
            {
                [SeriesWatchStatus.NotWatched] = Messages.SeriesNotWatched,
                [SeriesWatchStatus.Watching] = Messages.SeriesWatching,
                [SeriesWatchStatus.Watched] = Messages.SeriesWatched,
                [SeriesWatchStatus.StoppedWatching] = Messages.SeriesStoppedWatching
            };

            this.stringToStatus = statusToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(SeriesWatchStatus) && toType == typeof(string) ||
               fromType == typeof(string) && toType == typeof(SeriesWatchStatus)
                ? 10
                : 0;

        public bool TryConvert(object from, Type toType, object conversionHint, out object? result)
        {
            switch (from)
            {
                case SeriesWatchStatus status:
                    result = this.statusToString[status];
                    return true;
                case string str:
                    result = this.stringToStatus[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
