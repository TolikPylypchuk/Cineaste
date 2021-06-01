namespace Cineaste.Core.Models
{
    public sealed record CreateFileModel(string File, string ListName);

    public sealed record OpenFileModel(string File)
    {
        public bool IsExternal { get; init; }
    }
}
