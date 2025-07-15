using Nito.Comparers;

namespace Cineaste.Client.Store.ListPage;

public sealed class ListItemContainer(
    ImmutableSortedSet<ListItemModel> items,
    ImmutableDictionary<Guid, ListItemModel> itemsById,
    ImmutableDictionary<(Guid, int), ListItemModel> itemsByFranchiseItem,
    CultureInfo culture)
{
    public static readonly ListItemContainer Empty = new(
        [],
        ImmutableDictionary.Create<Guid, ListItemModel>(),
        ImmutableDictionary.Create<(Guid, int), ListItemModel>(),
        CultureInfo.InvariantCulture);

    private static readonly IComparer<ListItemModel> ComparerByYear = ComparerBuilder.For<ListItemModel>()
        .OrderBy(item => item.StartYear, descending: false)
        .ThenBy(item => item.EndYear, descending: false);

    private readonly ImmutableSortedSet<ListItemModel> items = items;
    private readonly ImmutableDictionary<Guid, ListItemModel> itemsById = itemsById;
    private readonly ImmutableDictionary<(Guid, int), ListItemModel> itemsByFranchiseItem = itemsByFranchiseItem;
    private readonly CultureInfo culture = culture;

    public IReadOnlyCollection<ListItemModel> Items =>
        this.items;

    public ListItemModel this[Guid id] =>
        this.itemsById[id];

    public ListItemModel this[Guid franchiseId, int sequenceNumber] =>
        this.itemsByFranchiseItem[(franchiseId, sequenceNumber)];

    public static ListItemContainer Create(ListModel list)
    {
        var culture = CultureInfo.GetCultureInfo(list.Config.Culture);

        var itemsById = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableDictionary(item => item.Id, item => item);

        var itemsByFranchiseItem = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .Where(item => item.FranchiseItem is not null)
            .ToImmutableDictionary(
                item => (item.FranchiseItem!.FranchiseId, item.FranchiseItem!.SequenceNumber), item => item);

        var comparer = new ListItemTitleComparer(
            culture,
            ComparerByYear,
            id => itemsById[id],
            item => item.Title);

        var items = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableSortedSet(comparer);

        return new(items, itemsById, itemsByFranchiseItem, culture);
    }

    public ListItemContainer AddItem(ListItemModel item)
    {
        var newItemsById = this.itemsById.Add(item.Id, item);
        var newItemsByFranchiseItem = item.FranchiseItem is not null
            ? this.itemsByFranchiseItem.Add((item.FranchiseItem.FranchiseId, item.FranchiseItem.SequenceNumber), item)
            : this.itemsByFranchiseItem;

        var newComparer = new ListItemTitleComparer(
            this.culture,
            ComparerByYear,
            id => newItemsById[id],
            item => item.Title);

        var itemsBuilder = this.items.ToBuilder();
        itemsBuilder.KeyComparer = newComparer;
        itemsBuilder.Add(item);

        return new(itemsBuilder.ToImmutable(), newItemsById, newItemsByFranchiseItem, this.culture);
    }

    public ListItemContainer UpdateItem(ListItemModel item)
    {
        var itemsByIdBuilder = this.itemsById.ToBuilder();
        itemsByIdBuilder.Remove(item.Id);
        itemsByIdBuilder.Add(item.Id, item);

        var newItemsById = itemsByIdBuilder.ToImmutable();

        var itemsByFranchiseItemBuilder = this.itemsByFranchiseItem.ToBuilder();

        if (item.FranchiseItem is not null)
        {
            var key = (item.FranchiseItem.FranchiseId, item.FranchiseItem.SequenceNumber);
            itemsByFranchiseItemBuilder.Remove(key);
            itemsByFranchiseItemBuilder.Add(key, item);
        }

        var newItemsByFranchiseItem = itemsByFranchiseItemBuilder.ToImmutable();

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

        return new(newItems, newItemsById, newItemsByFranchiseItem, this.culture);
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

        return new(itemsBuilder.ToImmutable(), newItemsById, this.itemsByFranchiseItem, this.culture);
    }
}
