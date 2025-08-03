namespace Cineaste.Validation;

public interface IPosterValidator
{
    void ValidateContentType(string contentType);
}

public sealed class PosterValidator : IPosterValidator
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
