namespace Cineaste.Shared;

public sealed record SimpleListModel(Guid Id, string Name);

public sealed record ListModel(Guid Id, string Name, List<ListItemModel> Items);

public record ListItemModel(
    Guid Id,
    ListItemType Type,
    string Number,
    string Title,
    string OriginalTitle,
    string Years,
    string Color);

public enum ListItemType { Movie, Series, Franchise }
