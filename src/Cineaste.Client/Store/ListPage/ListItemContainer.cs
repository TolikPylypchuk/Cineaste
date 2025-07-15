namespace Cineaste.Client.Store.ListPage;

public sealed class ListItemContainer(
    ImmutableList<ListItemModel> items,
    ImmutableDictionary<Guid, ListItemModel> itemsById,
    ImmutableDictionary<(Guid, int), ListItemModel> itemsByFranchiseItem)
{
    public static readonly ListItemContainer Empty = new(
        [],
        ImmutableDictionary.Create<Guid, ListItemModel>(),
        ImmutableDictionary.Create<(Guid, int), ListItemModel>());

    private readonly ImmutableList<ListItemModel> items = items;
    private readonly ImmutableDictionary<Guid, ListItemModel> itemsById = itemsById;
    private readonly ImmutableDictionary<(Guid, int), ListItemModel> itemsByFranchiseItem = itemsByFranchiseItem;

    public IReadOnlyCollection<ListItemModel> Items =>
        this.items;

    public ListItemModel this[Guid id] =>
        this.itemsById[id];

    public ListItemModel this[Guid franchiseId, int sequenceNumber] =>
        this.itemsByFranchiseItem[(franchiseId, sequenceNumber)];

    public static ListItemContainer Create(ListModel list)
    {
        var itemsById = list.Items
            .ToImmutableDictionary(item => item.Id, item => item);

        var itemsByFranchiseItem = list.Items
            .Where(item => item.FranchiseItem is not null)
            .ToImmutableDictionary(
                item => (item.FranchiseItem!.FranchiseId, item.FranchiseItem!.SequenceNumber), item => item);

        return new(list.Items, itemsById, itemsByFranchiseItem);
    }

    public ListItemContainer AddItem(ListItemModel item) =>
        this;

    public ListItemContainer UpdateItem(ListItemModel item) =>
        this;

    public ListItemContainer RemoveItem(Guid id) =>
        this;
}
