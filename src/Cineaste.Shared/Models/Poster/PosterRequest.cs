namespace Cineaste.Shared.Models.Poster;

public sealed record PosterRequest(Stream Data, long ContentLength, string ContentType);
