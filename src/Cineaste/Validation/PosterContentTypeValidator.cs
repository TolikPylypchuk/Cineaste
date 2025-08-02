namespace Cineaste.Validation;

public sealed class PosterContentTypeValidator
{
    public void ValidateContentType(string contentType)
    {
        if (!PosterContentTypes.AcceptedImageContentTypes.Contains(contentType))
        {
            throw new UnsupportedInputException("MediaType", $"Media type {contentType} is not supported for posters")
                .WithProperty(contentType);
        }
    }
}
