using System.Collections.Generic;
using System.Globalization;

using MovieList.Data.Models;

namespace MovieList.Core.Preferences
{
    public interface ISettings
    {
        string DefaultSeasonTitle { get; set; }
        string DefaultSeasonOriginalTitle { get; set; }
        CultureInfo CultureInfo { get; set; }
        List<Kind> Kinds { get; }
        List<Tag> Tags { get; }
    }
}
