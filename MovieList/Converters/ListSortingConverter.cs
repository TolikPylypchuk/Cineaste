using System.Collections.Generic;

using MovieList.Core.ViewModels;
using MovieList.Properties;

namespace MovieList.Converters
{
    public sealed class ListSortingConverter : EnumConverter<ListSorting>
    {
        protected override Dictionary<ListSorting, string> CreateConverterDictionary()
            => new()
            {
                [ListSorting.ByTitle] = Messages.ListSortingByTitle,
                [ListSorting.ByOriginalTitle] = Messages.ListSortingByOriginalTitle,
                [ListSorting.ByTitleSimple] = Messages.ListSortingByTitleSimple,
                [ListSorting.ByOriginalTitleSimple] = Messages.ListSortingByOriginalTitleSimple,
                [ListSorting.ByYear] = Messages.ListSortingByYear
            };
    }
}
