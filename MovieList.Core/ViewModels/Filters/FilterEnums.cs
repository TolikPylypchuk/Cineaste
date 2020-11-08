using System.Collections.Immutable;

using static MovieList.Core.ViewModels.Filters.FilterOperation;
using static MovieList.Core.ViewModels.Filters.FilterType;

namespace MovieList.Core.ViewModels.Filters
{
    public enum FilterType
    {
        ByTitle,
        ByYear,
        ByKind,
        ByTags,
        ByIsStandalone,
        ByIsMovie,
        ByMovieIsWatched,
        ByMovieIsReleased,
        ByIsSeries,
        BySeriesWatchStatus,
        BySeriesReleaseStatus,
        BySeriesChannel,
        BySeriesNumberOfSeasons,
        BySeriesNumberOfEpisodes,
        BySeriesIsMiniseries,
        BySeriesIsAnthology
    }

    public enum FilterOperation
    {
        None,
        Is,
        StartsWith,
        EndsWith,
        LessThan,
        GreaterThan,
        Between,
        Include,
        Exclude,
        HaveCategory
    }

    public enum FilterComposition { And, Or }

    public static class FilterOperations
    {
        public static readonly ImmutableDictionary<FilterType, ImmutableList<FilterOperation>> ByType;

        static FilterOperations()
        {
            var textOperations = Operations(Is, StartsWith, EndsWith);
            var numberOperations = Operations(Is, LessThan, GreaterThan, Between);
            var onlyIs = Operations(Is);
            var tagOperations = Operations(Include, Exclude, HaveCategory);
            var noOperations = Operations(None);

            ByType = ImmutableDictionary.Create<FilterType, ImmutableList<FilterOperation>>()
                .Add(ByTitle, textOperations)
                .Add(ByYear, numberOperations)
                .Add(ByKind, onlyIs)
                .Add(ByTags, tagOperations)
                .Add(ByIsStandalone, noOperations)
                .Add(ByIsMovie, noOperations)
                .Add(ByIsSeries, noOperations)
                .Add(ByMovieIsWatched, noOperations)
                .Add(ByMovieIsReleased, noOperations)
                .Add(BySeriesWatchStatus, onlyIs)
                .Add(BySeriesReleaseStatus, onlyIs)
                .Add(BySeriesChannel, textOperations)
                .Add(BySeriesNumberOfSeasons, numberOperations)
                .Add(BySeriesNumberOfEpisodes, numberOperations)
                .Add(BySeriesIsMiniseries, noOperations)
                .Add(BySeriesIsAnthology, noOperations);
        }

        private static ImmutableList<FilterOperation> Operations(params FilterOperation[] ops) =>
            ImmutableList.Create<FilterOperation>().AddRange(ops);
    }
}
