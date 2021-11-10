namespace Cineaste;

public static class Extensions
{
    public static string Localized(this string text) =>
        Messages.ResourceManager.GetString(text) ?? text;

    public static Brush ToBrush(this Color color) =>
        new SolidColorBrush
        {
            Color = color
        };
}
