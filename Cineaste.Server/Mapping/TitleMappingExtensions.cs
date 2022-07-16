namespace Cineaste.Server.Mapping;

public static class TitleMappingExtensions
{
    public static TitleModel ToTitleModel(this Title title) =>
        new(title.Name, title.Priority);

    public static Title ToTitle(this TitleRequest title, bool isOriginal) =>
        new(title.Name, title.Priority, isOriginal);

    public static ImmutableList<TitleModel> ToTitleModels(this IEnumerable<Title> titles, bool isOriginal) =>
        titles
            .Where(title => title.IsOriginal == isOriginal)
            .Select(title => title.ToTitleModel())
            .OrderBy(title => title.Priority)
            .ToImmutableList();
}
