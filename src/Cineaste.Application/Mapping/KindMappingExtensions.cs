namespace Cineaste.Application.Mapping;

public static class KindMappingExtensions
{
    extension<TKind>(Kind<TKind> kind)
        where TKind : Kind<TKind>
    {
        public ListKindModel ToListKindModel() =>
            new(
                kind.Id.Value,
                kind.Name,
                kind.WatchedColor.HexValue,
                kind.NotWatchedColor.HexValue,
                kind.NotReleasedColor.HexValue,
                kind is MovieKind ? ListKindTarget.Movie : ListKindTarget.Series);
    }
}
