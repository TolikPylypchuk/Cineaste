namespace Cineaste.Shared.Validation.Poster;

public sealed class PosterImdbMediaRequestValidator : CineasteValidator<PosterImdbMediaRequest>
{
    public PosterImdbMediaRequestValidator()
        : base("Poster.ImdbMedia")
    {
        this.RuleFor(req => req.Url)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Url, Empty));

        this.RuleFor(req => req.Url)
            .Must(url => String.IsNullOrWhiteSpace(url) ||
                Uri.IsWellFormedUriString(url, UriKind.Absolute) && this.IsHttpUri(new Uri(url)))
            .WithErrorCode(this.ErrorCode(req => req.Url, Invalid));
    }

    private bool IsHttpUri(Uri uri)
    {
        string scheme = uri.Scheme.ToLowerInvariant();
        return scheme == "http" || scheme == "https";
    }
}
