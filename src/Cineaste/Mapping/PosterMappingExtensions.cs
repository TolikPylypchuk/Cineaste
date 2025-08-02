using Cineaste.Shared.Models.Poster;

namespace Cineaste.Mapping;

public static class PosterMappingExtensions
{
    public static PosterModel ToPosterModel(this IPoster poster) =>
        new(poster.Data, poster.ContentType);
}
