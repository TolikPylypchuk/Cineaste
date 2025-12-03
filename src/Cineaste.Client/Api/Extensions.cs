namespace Cineaste.Client.Api;

public static class Extensions
{
    extension(IBrowserFile file)
    {
        public StreamPart ToStreamPart(string? name = null) =>
            new(file.OpenReadStream(), file.Name, file.ContentType, name);
    }
}
