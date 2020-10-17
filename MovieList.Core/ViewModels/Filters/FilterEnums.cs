using System;

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
        Series
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

    [Flags]
    public enum ListItemType
    {
        Movie,
        Series,
        Franchise
    }
}
