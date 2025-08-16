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

        this.RuleFor(req => req.Url)
            .Must(url =>
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return true;
                }

                var path = new Uri(url).AbsolutePath;
                return !String.IsNullOrEmpty(path) && path != "/";
            })
            .WithErrorCode(this.ErrorCode(req => req.Url, "NoPath"));
    }

    private bool IsHttpUri(Uri uri)
    {
        string scheme = uri.Scheme.ToLowerInvariant();
        return scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps;
    }
}
