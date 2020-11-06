using System.Collections.Generic;
using System.ComponentModel;

using MovieList.Properties;

namespace MovieList.Converters
{
    public sealed class ListSortDirectionConverter : EnumConverter<ListSortDirection>
    {
        protected override Dictionary<ListSortDirection, string> CreateConverterDictionary()
            => new()
            {
                [ListSortDirection.Ascending] = Messages.ListSortDirectionAscending,
                [ListSortDirection.Descending] = Messages.ListSortDirectionDescending
            };
    }
}
