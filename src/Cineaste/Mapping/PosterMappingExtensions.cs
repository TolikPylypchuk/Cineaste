using Cineaste.Models;
using Cineaste.Shared.Models.Poster;

namespace Cineaste.Mapping;

public static class PosterMappingExtensions
{
    public static PosterContentModel ToPosterModel(this IPoster poster) =>
        new(poster.Data, poster.ContentType);
}
