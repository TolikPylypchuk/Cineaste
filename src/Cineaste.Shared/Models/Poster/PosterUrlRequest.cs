using Cineaste.Shared.Validation.Poster;

namespace Cineaste.Shared.Models.Poster;

public sealed record PosterUrlRequest(string Url) : IValidatable<PosterUrlRequest>
{
    public static IValidator<PosterUrlRequest> Validator { get; } = new PosterUrlRequestValidator();
}
