using System.ComponentModel;

namespace Cineaste.Core.Domain;

public sealed class ListSortingConfiguration(
    ListSortOrder defaultFirstSortOrder,
    ListSortDirection defaultFirstSortDirection,
    ListSortOrder defaultSecondSortOrder,
    ListSortDirection defaultSecondSortDirection)
{
    public ListSortOrder DefaultFirstSortOrder { get; set; } = defaultFirstSortOrder;
    public ListSortDirection DefaultFirstSortDirection { get; set; } = defaultFirstSortDirection;

    public ListSortOrder DefaultSecondSortOrder { get; set; } = defaultSecondSortOrder;
    public ListSortDirection DefaultSecondSortDirection { get; set; } = defaultSecondSortDirection;

    public static ListSortingConfiguration CreateDefault() =>
        new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending);
}
