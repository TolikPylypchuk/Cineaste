using System;
using System.Diagnostics;

using Avalonia.Media;

using Cineaste.Properties;

using static Cineaste.Core.Util;

namespace Cineaste
{
    public static class Extensions
    {
        public static string Localized(this string text) =>
            Messages.ResourceManager.GetString(text) ?? text;

        public static Brush ToBrush(this Color color) =>
            new SolidColorBrush
            {
                Color = color
            };

        public static void OpenInBrowser(this Uri uri) =>
            PlatformDependent(
                windows: () => Process.Start(
                    new ProcessStartInfo { FileName = uri.ToString(), UseShellExecute = true }),
                macos: () => Process.Start("open", uri.ToString()),
                linux: () => Process.Start("xdg-open", uri.ToString()));
    }
}
