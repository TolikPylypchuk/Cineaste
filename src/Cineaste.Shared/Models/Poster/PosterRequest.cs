using System.Text.Json.Serialization;

using Cineaste.Shared.Validation.Poster;

namespace Cineaste.Shared.Models.Poster;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PosterUrlRequest), typeDiscriminator: "Url")]
[JsonDerivedType(typeof(PosterImdbMediaRequest), typeDiscriminator: "ImdbMedia")]
public abstract record PosterRequestBase;

public sealed record PosterUrlRequest(string Url) : PosterRequestBase, IValidatable<PosterUrlRequest>
{
    public static IValidator<PosterUrlRequest> Validator { get; } = new PosterUrlRequestValidator();
}

public sealed record PosterImdbMediaRequest(string Url) : PosterRequestBase, IValidatable<PosterImdbMediaRequest>
{
    public static IValidator<PosterImdbMediaRequest> Validator { get; } = new PosterImdbMediaRequestValidator();
}
