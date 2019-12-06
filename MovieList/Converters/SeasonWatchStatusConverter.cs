using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class SeasonWatchStatusConverter : IBindingTypeConverter
    {
        private readonly Dictionary<SeasonWatchStatus, string> statusToString;
        private readonly Dictionary<string, SeasonWatchStatus> stringToStatus;

        public SeasonWatchStatusConverter()
        {
            this.statusToString = new Dictionary<SeasonWatchStatus, string>
            {
                [SeasonWatchStatus.NotWatched] = Messages.SeriesNotWatched,
                [SeasonWatchStatus.Watching] = Messages.SeriesWatching,
                [SeasonWatchStatus.Watched] = Messages.SeriesWatched,
                [SeasonWatchStatus.StoppedWatching] = Messages.SeriesStoppedWatching
            };

            this.stringToStatus = statusToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(SeasonWatchStatus) && toType == typeof(string) ||
               fromType == typeof(string) && toType == typeof(SeasonWatchStatus)
                ? 10
                : 0;

        public bool TryConvert(object from, Type toType, object conversionHint, out object? result)
        {
            switch (from)
            {
                case SeasonWatchStatus status:
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
