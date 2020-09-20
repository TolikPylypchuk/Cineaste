using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MovieList.Core
{
    public static class Extensions
    {
        public static string? NullIfEmpty(this string? str)
            => String.IsNullOrEmpty(str) ? null : str;

        public static string EmptyIfNull(this string? str)
            => String.IsNullOrEmpty(str) ? String.Empty : str;

        public static void DisposeAndRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : IDisposable
        {
            dictionary[key].Dispose();
            dictionary.Remove(key);
        }

        public static string GetMemberName(this Expression expression)
            => expression is LambdaExpression lambda && lambda.Body is MemberExpression member
                ? member.Member.Name
                : throw new NotSupportedException("Non-lambda expressions with member access are not supported.");
    }
}
