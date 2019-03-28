using System.Windows.Media;

namespace MovieList.Config
{
    public sealed class Configuration
    {
        public string DatabasePath { get; set; }

        public Color NotWatchedColor { get; set; }
        public Color NotReleasedColor { get; set; }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }
    }
}
