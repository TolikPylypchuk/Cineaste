using System.Drawing;

namespace MovieList
{
    public sealed class Settings
    {
        internal Settings(string databasePath, Color notWatchedColor, Color notReleasedColor)
        {
            this.DatabasePath = databasePath;
            this.NotWatchedColor = notWatchedColor;
            this.NotReleasedColor = notReleasedColor;
        }

        public string DatabasePath { get; set; }
        public Color NotWatchedColor { get; set; }
        public Color NotReleasedColor { get; set; }
    }
}
