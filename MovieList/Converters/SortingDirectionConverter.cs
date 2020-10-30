using System.Collections.Generic;

using MovieList.Core.ViewModels;
using MovieList.Properties;

namespace MovieList.Converters
{
    public sealed class SortingDirectionConverter : EnumConverter<SortingDirection>
    {
        protected override Dictionary<SortingDirection, string> CreateConverterDictionary()
            => new()
            {
                [SortingDirection.Ascending] = Messages.SortingDirectionAscending,
                [SortingDirection.Descending] = Messages.SortingDirectionDescending
            };
    }
}
