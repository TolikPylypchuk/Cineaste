using System;

namespace MovieList
{
    public static class ValidationExtensions
    {
        public static bool IsUrl(this string? str)
            => String.IsNullOrEmpty(str) ||
                str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase);
    }
}
