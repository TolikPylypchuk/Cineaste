namespace Cineaste.Core.Domain;

using System.ComponentModel;
using System.Globalization;

public enum ListSortOrder
{
    ByTitle,
    ByOriginalTitle,
    ByTitleSimple,
    ByOriginalTitleSimple,
    ByYear
}

public sealed class MovieSeriesList : DomainObject
{
    public string Name { get; set; } = String.Empty;

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    public string DefaultSeasonTitle { get; set; } = String.Empty;
    public string DefaultSeasonOriginalTitle { get; set; } = String.Empty;

    public ListSortOrder DefaultFirstSortOrder { get; set; } = ListSortOrder.ByTitle;
    public ListSortDirection DefaultFirstSortDirection { get; set; } = ListSortDirection.Ascending;

    public ListSortOrder DefaultSecondSortOrder { get; set; } = ListSortOrder.ByYear;
    public ListSortDirection DefaultSecondSortDirection { get; set; } = ListSortDirection.Ascending;

    public List<Movie> Movies { get; set; } = new();

    public List<Series> Series { get; set; } = new();

    public List<Franchise> Franchises { get; set; } = new();
}
