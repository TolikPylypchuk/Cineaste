using System.Drawing;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Config
{
    public sealed class Configuration
    {
        public string DatabasePath { get; set; }

        public Color NotWatchedColor { get; set; }
        public Color NotReleasedColor { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
