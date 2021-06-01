using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using MovieList.Data;
using MovieList.Data.Models;

namespace MovieList.Core.Preferences
{
    public interface ISettings
    {
        string DefaultSeasonTitle { get; set; }
        string DefaultSeasonOriginalTitle { get; set; }

        List<Kind> Kinds { get; }
        List<Tag> Tags { get; }

        CultureInfo CultureInfo { get; set; }

        ListSortOrder DefaultFirstSortOrder { get; set; }
        ListSortOrder DefaultSecondSortOrder { get; set; }

        ListSortDirection DefaultFirstSortDirection { get; set; }
        ListSortDirection DefaultSecondSortDirection { get; set; }
    }
}
