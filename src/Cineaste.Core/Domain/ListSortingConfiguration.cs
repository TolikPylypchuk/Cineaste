using System.ComponentModel;

namespace Cineaste.Core.Domain;

public sealed class ListSortingConfiguration
{
    public ListSortOrder FirstSortOrder { get; set; }
    public ListSortOrder SecondSortOrder { get; set; }

    public ListSortDirection SortDirection { get; set; }

    public ListSortingConfiguration(
        ListSortOrder firstSortOrder,
        ListSortOrder secondSortOrder,
        ListSortDirection sortDirection)
    {
        if (firstSortOrder != ListSortOrder.ByYear && secondSortOrder != ListSortOrder.ByYear)
        {
            throw new ArgumentOutOfRangeException(
                nameof(secondSortOrder), "Second sort order must be by year if the first sort order is not by year");
        }

        if (firstSortOrder == ListSortOrder.ByYear &&
            (secondSortOrder is not (ListSortOrder.ByTitleSimple or ListSortOrder.ByOriginalTitleSimple)))
        {
            throw new ArgumentOutOfRangeException(
                nameof(secondSortOrder),
                "Second sort order must be by simple title (original or not) if the first sort order is by year");
        }

        this.FirstSortOrder = firstSortOrder;
        this.SecondSortOrder = secondSortOrder;
        this.SortDirection = sortDirection;
    }

    public static ListSortingConfiguration CreateDefault() =>
        new(ListSortOrder.ByTitle, ListSortOrder.ByYear, ListSortDirection.Ascending);
}
