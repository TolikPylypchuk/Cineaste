namespace Cineaste.Shared.Validation;

public static class PosterContentTypes
{
    public const string ImageApng = "image/apng";
    public const string ImageAvif = "image/avif";
    public const string ImageGif = "image/gif";
    public const string ImageJpeg = "image/jpeg";
    public const string ImagePng = "image/png";
    public const string ImageSvg = "image/svg+xml";
    public const string ImageWebp = "image/webp";

    public static readonly ImmutableHashSet<string> AcceptedImageContentTypes =
    [
        ImageApng,
        ImageAvif,
        ImageGif,
        ImageJpeg,
        ImagePng,
        ImageSvg,
        ImageWebp
    ];

    public static readonly ImmutableList<string> AcceptedImageFileExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".apng",
        ".avif",
        ".gif",
        ".svg",
        ".webp",
        ".jfif",
        ".pjpeg",
        ".pjp",
    ];
}
