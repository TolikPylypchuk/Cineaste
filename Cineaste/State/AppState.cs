namespace Cineaste.State;

public sealed class AppState
{
    public double WindowWidth { get; set; }
    public double WindowHeight { get; set; }
    public double WindowX { get; set; }
    public double WindowY { get; set; }
    public bool IsWindowMaximized { get; set; }
    public bool IsInitialized { get; set; }
}
