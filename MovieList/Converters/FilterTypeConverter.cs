using System.Collections.Generic;

using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

namespace MovieList.Converters
{
    public sealed class FilterTypeConverter : EnumConverter<FilterType>
    {
        protected override Dictionary<FilterType, string> CreateConverterDictionary()
            => new()
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
    }
}
