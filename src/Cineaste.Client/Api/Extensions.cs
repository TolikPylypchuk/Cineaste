namespace Cineaste.Client.Api;

public static class Extensions
{
    public static StreamPart ToStreamPart(this IBrowserFile file, string? name = null) =>
        new(file.OpenReadStream(), file.Name, file.ContentType, name);
}
