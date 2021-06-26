using System;

namespace Cineaste
{
    public static class Util
    {
        public static string GetUnixHomeFolder() =>
            Environment.GetEnvironmentVariable("HOME") ?? String.Empty;
    }
}
