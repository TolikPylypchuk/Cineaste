namespace Cineaste.Server.Mappers;

public static class KindMappingExtensions
{
    public static SimpleKindModel ToSimpleKindModel<TKind>(this Kind<TKind> kind)
        where TKind : Kind<TKind> =>
        new(kind.Id.Value, kind.Name, kind is MovieKind ? ListKindTarget.Movie : ListKindTarget.Series);

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
