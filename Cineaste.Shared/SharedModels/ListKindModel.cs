namespace Cineaste.Shared.SharedModels;

public sealed record ListKindModel(
    Guid Id,
    string Name,
    string WatchedColor,
    string NotWatchedColor,
    string NotReleasedColor,
    ListKindTarget Target);

public enum ListKindTarget { Movie, Series }
