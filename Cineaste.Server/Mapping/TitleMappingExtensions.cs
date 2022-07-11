namespace Cineaste.Server.Mapping;

public static class TitleMappingExtensions
{
    public static TitleModel ToTitleModel(this Title title) =>
        new(title.Name, title.Priority);

    public static Title ToTitle(this TitleRequest title, bool isOriginal) =>
        new(title.Name, title.Priority, isOriginal);
}
