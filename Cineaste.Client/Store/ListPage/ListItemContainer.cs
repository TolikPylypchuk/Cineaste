namespace Cineaste.Client.Store.ListPage;

using Nito.Comparers;

[AutoConstructor]
public sealed partial class ListItemContainer
{
    public static readonly ListItemContainer Empty = new(
        ImmutableSortedSet.Create<ListItemModel>(),
        ImmutableDictionary.Create<Guid, ListItemModel>(),
        CultureInfo.InvariantCulture);

    private static readonly IComparer<ListItemModel> ComparerByYear = ComparerBuilder.For<ListItemModel>()
        .OrderBy(item => item.StartYear, descending: false)
        .ThenBy(item => item.EndYear, descending: false);

    private readonly ImmutableSortedSet<ListItemModel> items;
    private readonly ImmutableDictionary<Guid, ListItemModel> itemsById;
    private readonly CultureInfo culture;

    public IReadOnlyCollection<ListItemModel> Items =>
        this.items;

    public static ListItemContainer Create(ListModel list)
    {
        var culture = CultureInfo.GetCultureInfo(list.Config.Culture);

        var itemsById = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableDictionary(item => item.Id, item => item);

        var comparer = new ListItemTitleComparer(
            culture,
            ComparerByYear,
            id => itemsById[id],
            item => item.Title);

        var items = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableSortedSet(comparer);

        return new(items, itemsById, culture);
    }

    public ListItemContainer AddItem(ListItemModel item)
    {
        var newItemsById = this.itemsById.Add(item.Id, item);

        var newComparer = new ListItemTitleComparer(
            this.culture,
            ComparerByYear,
            id => newItemsById[id],
            item => item.Title);

        var newItems = this.items.Add(item).ToImmutableSortedSet(newComparer);

        return new(newItems, newItemsById, this.culture);
    }

    public ListItemContainer RemoveItem(Guid id)
    {
        var newItemsById = this.itemsById.Remove(id);

        var newComparer = new ListItemTitleComparer(
            this.culture,
            ComparerByYear,
            id => newItemsById[id],
            item => item.Title);

        var newItems = this.items
            .Where(item => item.Id != id)
            .ToImmutableSortedSet(newComparer);

        return new(newItems, newItemsById, this.culture);
    }
}
