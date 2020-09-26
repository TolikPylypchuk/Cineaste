using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MovieList.Core.Comparers
{
    public sealed class PropertyEqualityComparer<T, TProperty> : EqualityComparer<T>
    {
        private readonly Func<T, TProperty> propertyGetter;
        private readonly IEqualityComparer<TProperty> propertyComparer;

        public PropertyEqualityComparer(
            Func<T, TProperty> propertyGetter,
            IEqualityComparer<TProperty>? propertyComparer = null)
        {
            this.propertyGetter = propertyGetter;
            this.propertyComparer = propertyComparer ?? EqualityComparer<TProperty>.Default;
        }

        public override bool Equals(T? x, T? y)
            => (x, y) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                var (left, right) => this.propertyComparer.Equals(this.propertyGetter(left), this.propertyGetter(right))
            };

        public override int GetHashCode([DisallowNull] T x)
            => x is null ? 1 : this.propertyComparer.GetHashCode(this.propertyGetter(x)!);
    }
}
