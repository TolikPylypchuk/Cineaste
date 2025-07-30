namespace Cineaste.Shared.Models.Shared;

public sealed record ListKindModel(
    Guid Id,
    string Name,
    string WatchedColor,
    string NotWatchedColor,
    string NotReleasedColor,
    ListKindTarget Target): IIdentifyableModel;

public enum ListKindTarget { Movie, Series }
