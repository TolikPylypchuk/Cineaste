using System;
using System.Reactive.Linq;

using ReactiveUI.Validation.Helpers;

namespace MovieList
{
    public static class ValidationExtensions
    {
        public static IObservable<bool> Valid(this ValidationHelper rule)
            => rule.ValidationChanged.Select(state => state.IsValid);

        public static bool IsUrl(this string? str)
            => String.IsNullOrEmpty(str) ||
                str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase);
    }
}
