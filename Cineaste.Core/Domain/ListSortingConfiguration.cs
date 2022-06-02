namespace Cineaste.Core.Domain;

using System.ComponentModel;

public sealed class ListSortingConfiguration
{
    public ListSortOrder DefaultFirstSortOrder { get; set; } = ListSortOrder.ByTitle;
    public ListSortDirection DefaultFirstSortDirection { get; set; } = ListSortDirection.Ascending;

    public ListSortOrder DefaultSecondSortOrder { get; set; } = ListSortOrder.ByYear;
    public ListSortDirection DefaultSecondSortDirection { get; set; } = ListSortDirection.Ascending;

    public ListSortingConfiguration(
        ListSortOrder defaultFirstSortOrder,
        ListSortDirection defaultFirstSortDirection,
        ListSortOrder defaultSecondSortOrder,
        ListSortDirection defaultSecondSortDirection)
    {
        this.DefaultFirstSortOrder = defaultFirstSortOrder;
        this.DefaultFirstSortDirection = defaultFirstSortDirection;
        this.DefaultSecondSortOrder = defaultSecondSortOrder;
        this.DefaultSecondSortDirection = defaultSecondSortDirection;
    }

    public static ListSortingConfiguration CreateDefault() =>
        new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending);
}
