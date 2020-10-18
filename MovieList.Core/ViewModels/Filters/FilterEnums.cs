using System.Collections.Immutable;

using static MovieList.Core.ViewModels.Filters.FilterOperation;
using static MovieList.Core.ViewModels.Filters.FilterType;

namespace MovieList.Core.ViewModels.Filters
{
    public enum FilterType
    {
        Title,
        Year,
        Kind,
        Tags,
        Standalone,
        Movie,
        MovieWatched,
        MovieReleased,
        Series,
        SeriesWatchStatus,
        SeriesReleaseStatus,
        SeriesChannel,
        SeriesNumberOfSeasons,
        SeriesNumberOfEpisodes,
        SeriesMiniseries,
        SeriesAnthology
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
                .Add(Title, textOperations)
                .Add(Year, numberOperations)
                .Add(Kind, onlyIs)
                .Add(Tags, tagOperations)
                .Add(Standalone, noOperations)
                .Add(Movie, noOperations)
                .Add(Series, noOperations)
                .Add(MovieWatched, noOperations)
                .Add(MovieReleased, noOperations)
                .Add(SeriesWatchStatus, onlyIs)
                .Add(SeriesReleaseStatus, onlyIs)
                .Add(SeriesChannel, textOperations)
                .Add(SeriesNumberOfSeasons, numberOperations)
                .Add(SeriesNumberOfEpisodes, numberOperations)
                .Add(SeriesMiniseries, noOperations)
                .Add(SeriesAnthology, noOperations);
        }

        private static ImmutableList<FilterOperation> Operations(params FilterOperation[] ops)
            => ImmutableList.Create<FilterOperation>().AddRange(ops);
    }
}
