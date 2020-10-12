using System;

namespace MovieList.Core.ViewModels.Filters
{
    public enum FilterType
    {
        Title,
        Year,
        Tags,
        Standalone
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
        HaveCategory,
        NoneOfCategory
    }

    [Flags]
    public enum ListItemType
    {
        Movie,
        Series,
        Franchise
    }
}
