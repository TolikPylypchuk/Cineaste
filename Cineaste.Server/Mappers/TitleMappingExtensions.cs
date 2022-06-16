namespace Cineaste.Server.Mappers;

public static class TitleMappingExtensions
{
    public static TitleModel ToTitleModel(this Title title) =>
        new(title.Name, title.Priority, title.IsOriginal);
}
