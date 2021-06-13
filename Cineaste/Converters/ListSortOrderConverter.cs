using System.Collections.Generic;

using Cineaste.Data;
using Cineaste.Properties;

namespace Cineaste.Converters
{
    public sealed class ListSortOrderConverter : EnumConverter<ListSortOrder>
    {
        protected override Dictionary<ListSortOrder, string> CreateConverterDictionary() =>
            new()
            {
                [ListSortOrder.ByTitle] = Messages.ListSortingByTitle,
                [ListSortOrder.ByOriginalTitle] = Messages.ListSortingByOriginalTitle,
                [ListSortOrder.ByTitleSimple] = Messages.ListSortingByTitleSimple,
                [ListSortOrder.ByOriginalTitleSimple] = Messages.ListSortingByOriginalTitleSimple,
                [ListSortOrder.ByYear] = Messages.ListSortingByYear
            };
    }
}
