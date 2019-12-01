using System;

namespace MovieList
{
    public static class Extensions
    {
        public static string? NullIfEmpty(this string? str)
            => String.IsNullOrEmpty(str) ? null : str;

        public static string EmptyIfNull(this string? str)
            => String.IsNullOrEmpty(str) ? String.Empty : str;
    }
}
