namespace Cineaste.Mapping;

public static class KindMappingExtensions
{
    public static ListKindModel ToListKindModel<TKind>(this Kind<TKind> kind)
        where TKind : Kind<TKind> =>
        new(
            kind.Id.Value,
            kind.Name,
            kind.WatchedColor.HexValue,
            kind.NotWatchedColor.HexValue,
            kind.NotReleasedColor.HexValue,
            kind is MovieKind ? ListKindTarget.Movie : ListKindTarget.Series);
}
