using System;

namespace MovieList
{
    public static class Extensions
    {
        public static string? NullIfEmpty(this string? str)
            => String.IsNullOrEmpty(str) ? null : str;
    }
}
