#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Config.Logging
{
    public class FileConfig
    {
        public string Path { get; set; }
        public bool Append { get; set; }
        public int FileSizeLimitBytes { get; set; }
        public int MaxRollingFiles { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
