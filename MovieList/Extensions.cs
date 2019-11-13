using MovieList.Properties;

namespace MovieList
{
    public static class Extensions
    {
        public static string Localized(this string text)
            => Messages.ResourceManager.GetString(text) ?? text;
    }
}
