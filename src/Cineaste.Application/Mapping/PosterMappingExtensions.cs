namespace Cineaste.Application.Mapping;

public static class PosterMappingExtensions
{
    public static BinaryContent ToPosterModel(this IPoster poster) =>
        new(poster.Data, poster.ContentType);
}
