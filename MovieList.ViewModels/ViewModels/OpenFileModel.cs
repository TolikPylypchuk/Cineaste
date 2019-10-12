namespace MovieList.ViewModels
{
    public sealed class OpenFileModel
    {
        public OpenFileModel(string file, bool isExternal = false)
        {
            this.File = file;
            this.IsExternal = isExternal;
        }

        public string File { get; }
        public bool IsExternal { get; }
    }
}
