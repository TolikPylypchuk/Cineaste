namespace Cineaste.Basic;

using System.Collections.ObjectModel;
using System.Linq.Expressions;

public static class Extensions
{
    public static string GetMemberName(this Expression expression) =>
        expression is LambdaExpression lambda && lambda.Body is MemberExpression member
            ? member.Member.Name
            : throw new NotSupportedException("Non-lambda expressions with member access are not supported");

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items) =>
        items.Where(item => item != null)!;

    public static IEnumerable<T> WhereValueNotNull<T>(this IEnumerable<T?> items)
        where T : struct =>
        items.Where(item => item.HasValue).Select(item => item!.Value);

    public static IEnumerable<T> WhereNotDefault<T>(this IEnumerable<T> items)
        where T : struct =>
        items.Where(item => !item.Equals(default(T)));

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items) =>
        new(items);

    public static bool Implies(this bool antecedent, bool consequent) =>
        !antecedent || consequent;
}
