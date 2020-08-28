using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class SeriesReleaseStatusConverter : IBindingTypeConverter
    {
        private readonly Dictionary<SeriesReleaseStatus, string> statusToString;
        private readonly Dictionary<string, SeriesReleaseStatus> stringToStatus;

        public SeriesReleaseStatusConverter()
        {
            this.statusToString = new Dictionary<SeriesReleaseStatus, string>
            {
                [SeriesReleaseStatus.NotStarted] = Messages.SeriesNotStarted,
                [SeriesReleaseStatus.Running] = Messages.SeriesRunning,
                [SeriesReleaseStatus.Finished] = Messages.SeriesFinished,
                [SeriesReleaseStatus.Cancelled] = Messages.SeriesCancelled,
                [SeriesReleaseStatus.Unknown] = Messages.SeriesUnknown
            };

            this.stringToStatus = statusToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(SeriesReleaseStatus) || toType == typeof(SeriesReleaseStatus)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case SeriesReleaseStatus status:
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
