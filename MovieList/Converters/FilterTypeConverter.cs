using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Core;
using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class FilterTypeConverter : IBindingTypeConverter, IEnumConverter<FilterType>
    {
        private readonly Dictionary<FilterType, string> filterTypeToString;
        private readonly Dictionary<string, FilterType> stringToFilterType;

        public FilterTypeConverter()
        {
            this.filterTypeToString = new Dictionary<FilterType, string>
            {
                [FilterType.ByTitle] = Messages.FilterTypeTitle,
                [FilterType.ByYear] = Messages.FilterTypeYear,
                [FilterType.ByKind] = Messages.FilterTypeKind,
                [FilterType.ByTags] = Messages.FilterTypeTags,
                [FilterType.ByIsStandalone] = Messages.FilterTypeStandalone,
                [FilterType.ByIsMovie] = Messages.FilterTypeMovie,
                [FilterType.ByIsSeries] = Messages.FilterTypeSeries,
                [FilterType.ByMovieIsWatched] = Messages.FilterTypeMovieWatched,
                [FilterType.ByMovieIsReleased] = Messages.FilterTypeMovieReleased,
                [FilterType.BySeriesWatchStatus] = Messages.FilterTypeSeriesWatchStatus,
                [FilterType.BySeriesReleaseStatus] = Messages.FilterTypeSeriesReleaseStatus,
                [FilterType.BySeriesChannel] = Messages.FilterTypeSeriesChannel,
                [FilterType.BySeriesNumberOfSeasons] = Messages.FilterTypeSeriesNumberOfSeasons,
                [FilterType.BySeriesNumberOfEpisodes] = Messages.FilterTypeSeriesNumberOfEpisodes,
                [FilterType.BySeriesIsMiniseries] = Messages.FilterTypeSeriesMiniseries,
                [FilterType.BySeriesIsAnthology] = Messages.FilterTypeSeriesAnthology
            };

            this.stringToFilterType = filterTypeToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(FilterType) || toType == typeof(FilterType)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case FilterType filterType:
                    result = this.filterTypeToString[filterType];
                    return true;
                case string str:
                    result = this.stringToFilterType[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }

        public string ToString(FilterType filterType)
            => this.filterTypeToString[filterType];

        public FilterType FromString(string str)
            => this.stringToFilterType.ContainsKey(str)
                ? this.stringToFilterType[str]
                : throw new ArgumentOutOfRangeException(nameof(str));
    }
}
