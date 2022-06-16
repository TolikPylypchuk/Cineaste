namespace Cineaste.Shared.SharedModels;

public sealed record ListKindModel(
    Guid Id,
    string Name,
    string WatchedColor,
    string NotWatchedColor,
    string NotReleasedColor,
    ListKindTarget Target);

public sealed record SimpleKindModel(Guid Id, string Name, ListKindTarget Target);

public enum ListKindTarget { Movie, Series }
