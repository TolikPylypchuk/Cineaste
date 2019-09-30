namespace MovieList.Preferences
{
    public class UIPreferences
    {
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public double WindowX { get; set; }
        public double WindowY { get; set; }
        public bool IsWindowMaximized { get; set; }

        public UIPreferences(
            double windowWidth,
            double windowHeight,
            double windowX,
            double windowY,
            bool isWindowMaximized)
        {
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
            this.WindowX = windowX;
            this.WindowY = windowY;
            this.IsWindowMaximized = isWindowMaximized;
        }
    }
}
