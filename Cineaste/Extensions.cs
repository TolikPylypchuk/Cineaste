using Cineaste.Properties;

namespace Cineaste
{
    public static class Extensions
    {
        public static string Localized(this string text) =>
            Messages.ResourceManager.GetString(text) ?? text;
    }
}
