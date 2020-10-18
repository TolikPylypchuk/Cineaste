using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Core;
using MovieList.Data.Models;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class SeasonReleaseStatusConverter : IBindingTypeConverter, IEnumConverter<SeasonReleaseStatus>
    {
        private readonly Dictionary<SeasonReleaseStatus, string> statusToString;
        private readonly Dictionary<string, SeasonReleaseStatus> stringToStatus;

        public SeasonReleaseStatusConverter()
        {
            this.statusToString = new Dictionary<SeasonReleaseStatus, string>
            {
                [SeasonReleaseStatus.NotStarted] = Messages.SeasonNotStarted,
                [SeasonReleaseStatus.Running] = Messages.SeasonRunning,
                [SeasonReleaseStatus.Hiatus] = Messages.SeasonHiatus,
                [SeasonReleaseStatus.Finished] = Messages.SeasonFinished,
                [SeasonReleaseStatus.Unknown] = Messages.SeasonUnknown
            };

            this.stringToStatus = statusToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(SeasonReleaseStatus) || toType == typeof(SeasonReleaseStatus)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case SeasonReleaseStatus status:
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

        public string ToString(SeasonReleaseStatus status)
            => this.statusToString[status];

        public SeasonReleaseStatus FromString(string str)
            => this.stringToStatus.ContainsKey(str)
                ? this.stringToStatus[str]
                : throw new ArgumentOutOfRangeException(nameof(str));
    }
}
