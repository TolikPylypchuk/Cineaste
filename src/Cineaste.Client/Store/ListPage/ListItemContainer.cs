using Nito.Comparers;

namespace Cineaste.Client.Store.ListPage;

public sealed class ListItemContainer(
    ImmutableSortedSet<ListItemModel> items,
    ImmutableDictionary<Guid, ListItemModel> itemsById,
    CultureInfo culture)
{
    public static readonly ListItemContainer Empty = new(
        [], ImmutableDictionary.Create<Guid, ListItemModel>(), CultureInfo.InvariantCulture);

    private static readonly IComparer<ListItemModel> ComparerByYear = ComparerBuilder.For<ListItemModel>()
        .OrderBy(item => item.StartYear, descending: false)
        .ThenBy(item => item.EndYear, descending: false);

    private readonly ImmutableSortedSet<ListItemModel> items = items;
    private readonly ImmutableDictionary<Guid, ListItemModel> itemsById = itemsById;
    private readonly CultureInfo culture = culture;

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

        var itemsBuilder = this.items.ToBuilder();
        itemsBuilder.KeyComparer = newComparer;
        itemsBuilder.Add(item);

        return new(itemsBuilder.ToImmutable(), newItemsById, this.culture);
    }

    public ListItemContainer UpdateItem(ListItemModel item)
    {
        var itemsByIdBuilder = this.itemsById.ToBuilder();
        itemsByIdBuilder.Remove(item.Id);
        itemsByIdBuilder.Add(item.Id, item);

        var newItemsById = itemsByIdBuilder.ToImmutable();

        var newComparer = new ListItemTitleComparer(
            this.culture,
            ComparerByYear,
            id => newItemsById[id],
            item => item.Title);

        var itemsBuilder = this.items.ToBuilder();
        itemsBuilder.Remove(itemsBuilder.First(i => i.Id == item.Id));
        itemsBuilder.Add(item);
        itemsBuilder.KeyComparer = newComparer;

        var newItems = itemsBuilder.ToImmutable();

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

        var itemsBuilder = this.items.ToBuilder();
        itemsBuilder.Remove(itemsBuilder.First(item => item.Id == id));
        itemsBuilder.KeyComparer = newComparer;

        return new(itemsBuilder.ToImmutable(), newItemsById, this.culture);
    }
}
