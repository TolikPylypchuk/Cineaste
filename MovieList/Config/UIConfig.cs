using System.Windows.Media;

namespace MovieList.Config
{
    public sealed class UIConfig
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public bool IsMaximized { get; set; }

        public Color NotWatchedColor { get; set; }
        public Color NotReleasedColor { get; set; }
    }
}
