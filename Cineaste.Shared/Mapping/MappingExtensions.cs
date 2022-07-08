namespace Cineaste.Shared.Mapping;

public static class MappingExtensions
{
    public static ListItemModel ToListItemModel(this MovieModel model) =>
        new(
            model.Id,
            ListItemType.Movie,
            true,
            model.DisplayNumber,
            model.Titles.First().Name,
            model.OriginalTitles.First().Name,
            model.Year,
            model.Year,
            model.IsWatched
                ? model.Kind.WatchedColor
                : model.IsReleased ? model.Kind.NotWatchedColor : model.Kind.NotReleasedColor,
            null);
}
