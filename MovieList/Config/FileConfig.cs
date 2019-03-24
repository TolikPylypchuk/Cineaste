namespace MovieList.Config
{
    public class FileConfig
    {
        public string Path { get; set; }
        public bool Append { get; set; }
        public int FileSizeLimitBytes { get; set; }
        public int MaxRollingFiles { get; set; }
    }
}
