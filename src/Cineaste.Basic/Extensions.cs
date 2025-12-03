using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Cineaste.Basic;

public static class Extensions
{
    extension(Expression expression)
    {
        public string GetMemberName() =>
            expression is LambdaExpression lambda && lambda.Body is MemberExpression member
                ? member.Member.Name
                : throw new NotSupportedException("Non-lambda expressions with member access are not supported");
    }

    extension<T>(IEnumerable<T?> items)
    {
        public IEnumerable<T> WhereNotNull() =>
            items.Where(item => item != null)!;
    }

    extension<T>(IEnumerable<T?> items)
        where T : struct
    {
        public IEnumerable<T> WhereValueNotNull() =>
            items.Where(item => item.HasValue).Select(item => item!.Value);
    }

    extension<T>(IEnumerable<T> items)
    {
        public ObservableCollection<T> ToObservableCollection() =>
            new(items);
    }

    extension<T>(IEnumerable<T> items)
        where T : struct
    {
        public IEnumerable<T> WhereNotDefault() =>
            items.Where(item => !item.Equals(default(T)));
    }

    extension(bool antecedent)
    {
        public bool Implies(bool consequent) =>
            !antecedent || consequent;
    }
}
