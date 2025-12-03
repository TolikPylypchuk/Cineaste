namespace Cineaste.Application.Mapping;

public static class PosterMappingExtensions
{
    extension(IPoster poster)
    {
        public BinaryContent ToPosterModel() =>
            new(poster.Data, poster.ContentType);
    }
}
